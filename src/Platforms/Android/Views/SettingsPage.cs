using Android.Content;
using Glyphy.Platforms.Android;
using Microsoft.Maui.ApplicationModel;
using System;

namespace Glyphy.Views
{
    public partial class SettingsPage
    {
        private void Android_Constructor()
        {
            AboutDeviceButton.Clicked += AboutDeviceButton_Clicked;
            DeveloperProcessOptionsButton.Clicked += DeveloperProcessOptionsButton_Clicked;
        }

        private void Android_ContentPage_Loaded(object sender, EventArgs e)
        {
            Header.Padding = new(Header.Padding.Left, /*Header.Padding.Top + */Helpers.StatusBarHeight, Header.Padding.Right, Header.Padding.Bottom);
            Padding = new(Padding.Left, Padding.Top, Padding.Right, /*Padding.Bottom + */Helpers.NavigationBarHeight);
        }

        private void AboutDeviceButton_Clicked(object? sender, EventArgs e)
        {
            Intent intent = new Intent(Android.Provider.Settings.ActionDeviceInfoSettings);
            //TODO: See if I can navigate directly to the "Software information" page.
            Platform.CurrentActivity?.StartActivity(intent);
        }

        private void DeveloperProcessOptionsButton_Clicked(object? sender, EventArgs e)
        {
            Intent intent = new Intent(Android.Provider.Settings.ActionApplicationDevelopmentSettings);
            //TODO: See if I can have this scroll down to the "Background process limit" option.
            Platform.CurrentActivity?.StartActivity(intent);
        }
    }
}
