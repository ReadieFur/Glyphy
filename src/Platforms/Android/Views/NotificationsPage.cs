using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Glyphy.Controls;
using Glyphy.Platforms.Android;

namespace Glyphy.Views
{
    public partial class NotificationsPage : ContentPage
    {
        private bool _isDisappearing = false;

        private void Android_Constructor()
        {
            Loaded += Android_NotificationsPage_Loaded;
            Disappearing += NotificationsPage_Disappearing;
        }

        private async void Android_NotificationsPage_Loaded(object? sender, EventArgs e)
        {
            //Check if the user has granted notification permissions.
            if (!AndroidHelpers.HasNotificationAccess)
            {
                if (!await AndroidHelpers.RequestNotificationAccess())
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
            }
            else
            {
                await LoadNotifications();
            }
        }

        private void NotificationsPage_Disappearing(object? sender, EventArgs e)
        {
            _isDisappearing = true;
        }

        private Task LoadNotifications()
        {
            return Task.Run(() =>
            {
                #region Per-App
                //Get a list of all installed applications.
                IList<ApplicationInfo> installedApps = Android.App.Application.Context.PackageManager!.GetInstalledApplications(PackageInfoFlags.MetaData);

                //Iterate through each app and only show apps that have the notification permission.
                //I wonder if there is a faster and more efficient way of doing this as things like the settings app and Nova launcher seem to be able to query this stuff extremely quickly.
                foreach (ApplicationInfo appInfo in installedApps)
                {
                    try
                    {
                        if (_isDisappearing)
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
                                bitmap = Bitmap.CreateBitmap(icon.IntrinsicWidth <= 0 ? 1 : icon.IntrinsicWidth, icon.IntrinsicHeight <= 0 ? 1 : icon.IntrinsicHeight, Bitmap.Config.Argb8888!);

                                if (bitmap is not null)
                                {
                                    Canvas canvas = new Canvas(bitmap!);
                                    icon.SetBounds(0, 0, canvas.Width, canvas.Height);
                                    icon.Draw(canvas);
                                }
                            }

                            if (bitmap is not null)
                            {
                                byte[] bitmapData = [];
                                using (MemoryStream stream = new())
                                {
                                    bitmap.Compress(Bitmap.CompressFormat.Png!, 100, stream);
                                    bitmapData = stream.ToArray();
                                }

                                iconSource = ImageSource.FromStream(() => new MemoryStream(bitmapData));
                            }
                        }

                        NotificationEntry entry = new(appInfo.PackageName, iconSource, appLabel);
                        Dispatcher.Dispatch(() => Entries.Add(entry));
                    }
                    catch { }
                }
                #endregion
            });
        }
    }
}
