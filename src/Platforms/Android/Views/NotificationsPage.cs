using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Glyphy.Animation;
using Glyphy.Configuration;
using Glyphy.Configuration.NotificationConfiguration;
using Glyphy.Controls;
using Glyphy.Platforms.Android.Services;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Helpers = Glyphy.Platforms.Android.Helpers;

namespace Glyphy.Views
{
    public partial class NotificationsPage : ContentPage
    {
        private Dictionary<Guid, string> cachedGlyphs = new();

        private bool IsDisappearing = false;

        private void Android_ContentPage_Loaded(object sender, EventArgs e)
        {
            Header.Padding = new(Header.Padding.Left, /*Header.Padding.Top + */Helpers.StatusBarHeight, Header.Padding.Right, Header.Padding.Bottom);
            Padding = new(Padding.Left, Padding.Top, Padding.Right, /*Padding.Bottom + */Helpers.NavigationBarHeight);

            //Check if the user has granted notification permissions.
            if (!Helpers.HasNotificationAccess)
            {
                Task.Run(async () =>
                {
                    if (!await Helpers.RequestNotificationAccess())
                    {
                        Dispatcher.Dispatch(async () =>
                        {
                            await Toast.Make("Notification access is required to use this feature.", ToastDuration.Long).Show();
                            await Navigation.PopAsync();
                        });
                    }
                    else
                    {
                        await LoadNotifications();
                    }
                });
            }
            else
            {
                _ = LoadNotifications();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            IsDisappearing = true;
        }

        private Task LoadNotifications()
        {
            return Task.Run(async () =>
            {
                foreach (Guid glyphID in Storage.GetAnimationIDs())
                {
                    SAnimation? glyph = Storage.LoadAnimation(glyphID).Result;
                    if (glyph is null)
                        continue;

                    cachedGlyphs.Add(glyphID, glyph.Value.Name);
                }

                #region Defaults
                Dispatcher.Dispatch(() => DefaultGlyphPicker.ItemsSource = cachedGlyphs.Values.ToList());

                bool hasDefaultValue = (await Storage.NotificationServiceSettings.GetCached()).Configuration.TryGetValue(NotificationLightingService.DEFAULT_KEY, out SNotificationConfiguration notificationConfiguration);
                if (hasDefaultValue && cachedGlyphs.ContainsKey(notificationConfiguration.AnimationID))
                {
                    Dispatcher.Dispatch(() =>
                    {
                        DefaultEnabledSwitch.IsToggled = true;
                        DefaultGlyphPicker.IsEnabled = true;
                        DefaultGlyphPicker.SelectedIndex = cachedGlyphs.Keys.ToList().IndexOf(notificationConfiguration.AnimationID);
                    });
                }
                else
                {
                    Dispatcher.Dispatch(() =>
                    {
                        DefaultEnabledSwitch.IsToggled = false;
                        DefaultGlyphPicker.IsEnabled = false;
                        //DefaultGlyphPicker.SelectedIndex = glyphs.Keys.ToList().IndexOf(Glyphy.Resources.Presets.Glyphs.OFF.Id);
                        DefaultGlyphPicker.SelectedIndex = -1;
                    });
                }

                DefaultEnabledSwitch.Toggled += DefaultEnabledSwitch_Toggled;
                DefaultGlyphPicker.SelectedIndexChanged += DefaultGlyphPicker_SelectedIndexChanged;

                Dispatcher.Dispatch(() => DefaultEnabledSwitch.IsEnabled = true);
                #endregion

                #region Per-App
                //Get a list of all installed applications.
#pragma warning disable CA1422 // Validate platform compatibility
                IList<ApplicationInfo> installedApps = Android.App.Application.Context.PackageManager!.GetInstalledApplications(PackageInfoFlags.MetaData);
#pragma warning restore CA1422

                //Iterate through each app and only show apps that have the notification permission.
                //I wonder if there is a faster and more efficient way of doing this as things like the settings app and Nova launcher seem to be able to query this stuff extremely quickly.
                foreach (ApplicationInfo appInfo in installedApps)
                {
                    try
                    {
                        if (IsDisappearing)
                            return;

                        if (appInfo.PackageName is null)
                            continue;

                        //For now only show packages with user friendly names (as a result this hides most, what I believe are, system services).
                        string appLabel = Android.App.Application.Context.PackageManager!.GetApplicationLabel(appInfo);
                        if (appLabel.ToLower() == appInfo.PackageName.ToLower())
                            continue;

                        //Check if app has permission to send notifications.
                        //TODO: In the future I would like to be able to get all app notification channels to then allow the user to set Glyphs per channel as opposed to per application.
                        if (Android.App.Application.Context.PackageManager.CheckPermission(Android.Manifest.Permission.PostNotifications, appInfo.PackageName) != Permission.Granted)
                            continue;

                        //Get the app's icon.
                        Drawable? icon = Android.App.Application.Context.PackageManager.GetApplicationIcon(appInfo.PackageName);
                        //Convert the app icon into an image source.
                        ImageSource? iconSource = null;
                        if (icon is not null)
                        {
                            //https://stackoverflow.com/questions/3035692/how-to-convert-a-drawable-to-a-bitmap
                            Bitmap? bitmap;
                            if (icon is BitmapDrawable bitmapDrawable)
                            {
                                bitmap = bitmapDrawable.Bitmap;
                            }
                            else
                            {
                                bitmap = Bitmap.CreateBitmap(icon.IntrinsicWidth <= 0 ? 1 : icon.IntrinsicWidth, icon.IntrinsicHeight <= 0 ? 1 : icon.IntrinsicHeight, Bitmap.Config.Argb8888);

                                if (bitmap is not null)
                                {
                                    Canvas canvas = new Canvas(bitmap!);
                                    icon.SetBounds(0, 0, canvas.Width, canvas.Height);
                                    icon.Draw(canvas);
                                }
                            }

                            if (bitmap is not null)
                            {
                                byte[] bitmapData = new byte[0];
                                using (MemoryStream stream = new())
                                {
                                    bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
                                    bitmapData = stream.ToArray();
                                }

                                iconSource = ImageSource.FromStream(() => new MemoryStream(bitmapData));
                            }
                        }

                        NotificationEntry entry = new(appInfo.PackageName, iconSource, appLabel, cachedGlyphs);
                        Dispatcher.Dispatch(() => Entries.Add(entry));
                    }
                    catch { }
                }
                #endregion
            });
        }

        private void DefaultEnabledSwitch_Toggled(object? sender, ToggledEventArgs e)
        {
            bool isToggled = DefaultEnabledSwitch.IsToggled;

            Task.Run(async () =>
            {
                SNotificationConfigurationRoot notificationServiceConfiguration = await Storage.NotificationServiceSettings.GetCached();

                DefaultGlyphPicker.IsEnabled = isToggled;

                if (notificationServiceConfiguration.Configuration.ContainsKey(NotificationLightingService.DEFAULT_KEY))
                {
                    SNotificationConfiguration configuration = notificationServiceConfiguration.Configuration[NotificationLightingService.DEFAULT_KEY];
                    configuration.Enabled = e.Value;
                    notificationServiceConfiguration.Configuration[NotificationLightingService.DEFAULT_KEY] = configuration;
                }
                else
                {
                    notificationServiceConfiguration.Configuration.Add(NotificationLightingService.DEFAULT_KEY, new()
                    {
                        PackageName = NotificationLightingService.DEFAULT_KEY,
                        Enabled = e.Value,
                        AnimationID = Glyphy.Resources.Presets.Glyphs.OFF.Id
                    });
                }

                await Storage.NotificationServiceSettings.Save(notificationServiceConfiguration);
            });
        }

        private void DefaultGlyphPicker_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (DefaultGlyphPicker.SelectedIndex < 0 || DefaultGlyphPicker.SelectedIndex > cachedGlyphs.Count)
                return;
            Guid selectedAnimationID = cachedGlyphs.ElementAt(DefaultGlyphPicker.SelectedIndex).Key;

            Task.Run(async () =>
            {
                SNotificationConfigurationRoot notificationServiceConfiguration = await Storage.NotificationServiceSettings.GetCached();

                if (notificationServiceConfiguration.Configuration.ContainsKey(NotificationLightingService.DEFAULT_KEY))
                {
                    SNotificationConfiguration configuration = notificationServiceConfiguration.Configuration[NotificationLightingService.DEFAULT_KEY];
                    configuration.AnimationID = selectedAnimationID;
                    notificationServiceConfiguration.Configuration[NotificationLightingService.DEFAULT_KEY] = configuration;
                }
                else
                {
                    notificationServiceConfiguration.Configuration.Add(NotificationLightingService.DEFAULT_KEY, new()
                    {
                        PackageName = NotificationLightingService.DEFAULT_KEY,
                        Enabled = true,
                        AnimationID = selectedAnimationID
                    });
                }

                await Storage.NotificationServiceSettings.Save(notificationServiceConfiguration);
            });
        }
    }
}
