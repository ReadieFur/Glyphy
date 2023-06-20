using Glyphy.Configuration;
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

        bool hasStoredValue = Storage.GetCachedNotificationServiceConfiguration().Result.TryGetValue(packageName, out Guid storedGlyphID);
        if (hasStoredValue && glyphs.ContainsKey(storedGlyphID))
        {
            EnabledSwitch.IsToggled = true;
            GlyphPicker.IsEnabled = true;
            GlyphPicker.SelectedIndex = glyphs.Keys.ToList().IndexOf(storedGlyphID);
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
        bool isToggled = EnabledSwitch.IsToggled;

        Task.Run(async () =>
        {
            //This conversion of the dictionary does take time but I'm willing to accept that as it's a safety feature (at least for now) to not have the cached value be directly mutable.
            Dictionary<string, Guid> notificationServiceConfiguration = (await Storage.GetCachedNotificationServiceConfiguration()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            GlyphPicker.IsEnabled = isToggled;

            if (isToggled)
            {
                //TODO: Store these in an object so they can be disabled but keep track of what animation they were set to.
                //GlyphPicker.SelectedIndex = glyphs.Keys.ToList().IndexOf(GET_DEFAULT);

                if (notificationServiceConfiguration.ContainsKey(PackageName))
                    notificationServiceConfiguration[PackageName] = Glyphy.Resources.Presets.Glyphs.OFF.Id;
                else
                    notificationServiceConfiguration.Add(PackageName, Glyphy.Resources.Presets.Glyphs.OFF.Id);
            }
            else
            {
                GlyphPicker.SelectedIndex = -1;

                if (notificationServiceConfiguration.ContainsKey(PackageName))
                    notificationServiceConfiguration.Remove(PackageName);
            }

            await Storage.SaveNotificationServiceConfiguration(notificationServiceConfiguration);
        });
    }

    private void GlyphPicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (GlyphPicker.SelectedIndex < 0 || GlyphPicker.SelectedIndex > Glyphs.Count)
            return;
        Guid selectedAnimation = Glyphs.ElementAt(GlyphPicker.SelectedIndex).Key;

        Task.Run(async () =>
        {
            //This conversion of the dictionary does take time but I'm willing to accept that as it's a safety feature (at least for now) to not have the cached value be directly mutable.
            Dictionary<string, Guid> notificationServiceConfiguration = (await Storage.GetCachedNotificationServiceConfiguration()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            if (notificationServiceConfiguration.ContainsKey(PackageName))
                notificationServiceConfiguration[PackageName] = selectedAnimation;
            else
                notificationServiceConfiguration.Add(PackageName, selectedAnimation);

            await Storage.SaveNotificationServiceConfiguration(notificationServiceConfiguration);
        });
    }
}
