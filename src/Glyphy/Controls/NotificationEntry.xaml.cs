using Glyphy.Configuration;
using Glyphy.Configuration.NotificationConfiguration;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Glyphy.Controls;

public partial class NotificationEntry : ContentView
{
    public readonly string PackageName;
    private readonly Dictionary<Guid, string> Glyphs;

    [Obsolete("Parameterless constructor only exists for the XAML preview.", true)]
    public NotificationEntry()
	{
		InitializeComponent();
	}

	public NotificationEntry(string packageName, ImageSource? appIcon, string appLabel, Dictionary<Guid, string> glyphs)
	{
        InitializeComponent();

        PackageName = packageName;
        Glyphs = glyphs;

        AppIcon.Source = appIcon;
        NameLabel.Text = appLabel;

        GlyphPicker.ItemsSource = glyphs.Values.ToList();

        bool hasStoredValue = Storage.NotificationServiceSettings.GetCached().Result.Configuration.TryGetValue(packageName, out SNotificationConfiguration notificationConfiguration);
        if (hasStoredValue && glyphs.ContainsKey(notificationConfiguration.AnimationID))
        {
            EnabledSwitch.IsToggled = true;
            GlyphPicker.IsEnabled = true;
            GlyphPicker.SelectedIndex = glyphs.Keys.ToList().IndexOf(notificationConfiguration.AnimationID);
        }
        else
        {
            EnabledSwitch.IsToggled = false;
            GlyphPicker.IsEnabled = false;
            //GlyphPicker.SelectedIndex = glyphs.Keys.ToList().IndexOf(Glyphy.Resources.Presets.Glyphs.OFF.Id);
            GlyphPicker.SelectedIndex = -1;
        }

        EnabledSwitch.Toggled += EnabledSwitch_Toggled;
        GlyphPicker.SelectedIndexChanged += GlyphPicker_SelectedIndexChanged;

        EnabledSwitch.IsEnabled = true;
    }

    private void EnabledSwitch_Toggled(object? sender, ToggledEventArgs e)
    {
        Task.Run(async () =>
        {
            SNotificationConfigurationRoot notificationServiceConfiguration = await Storage.NotificationServiceSettings.GetCached();

            GlyphPicker.IsEnabled = e.Value;

            if (notificationServiceConfiguration.Configuration.ContainsKey(PackageName))
            {
                SNotificationConfiguration configuration = notificationServiceConfiguration.Configuration[PackageName];
                configuration.Enabled = e.Value;
                notificationServiceConfiguration.Configuration[PackageName] = configuration;
            }
            else
            {
                notificationServiceConfiguration.Configuration.Add(PackageName, new()
                {
                    PackageName = PackageName,
                    Enabled = e.Value,
                    AnimationID = Glyphy.Resources.Presets.Glyphs.OFF.Id
                });
            }

            await Storage.NotificationServiceSettings.Save(notificationServiceConfiguration);
        });
    }

    private void GlyphPicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (GlyphPicker.SelectedIndex < 0 || GlyphPicker.SelectedIndex > Glyphs.Count)
            return;
        Guid selectedAnimationID = Glyphs.ElementAt(GlyphPicker.SelectedIndex).Key;

        Task.Run(async () =>
        {
            SNotificationConfigurationRoot notificationServiceConfiguration = await Storage.NotificationServiceSettings.GetCached();

            if (notificationServiceConfiguration.Configuration.ContainsKey(PackageName))
            {
                SNotificationConfiguration configuration = notificationServiceConfiguration.Configuration[PackageName];
                configuration.AnimationID = selectedAnimationID;
                notificationServiceConfiguration.Configuration[PackageName] = configuration;
            }
            else
            {
                notificationServiceConfiguration.Configuration.Add(PackageName, new()
                {
                    PackageName = PackageName,
                    Enabled = true,
                    AnimationID = selectedAnimationID
                });
            }

            await Storage.NotificationServiceSettings.Save(notificationServiceConfiguration);
        });
    }
}
