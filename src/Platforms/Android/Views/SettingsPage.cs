using Android.Content;
using Android.Provider;

namespace Glyphy.Views
{
    public partial class SettingsPage : ContentPage
    {
        private void Android_Constructor()
        {
            //Hide 'About Device' section if developer mode is enabled already.
            try
            {
                //https://developer.android.com/reference/android/provider/Settings.Global
                bool developerModeEnabled = Settings.Global.GetInt(Android.App.Application.Context.ContentResolver, Settings.Global.DevelopmentSettingsEnabled, 0) == 1;
                if (developerModeEnabled)
                    AboutDeviceSection.IsVisible = false;
            }
            catch {}

            AboutDeviceButton.Clicked += AboutDeviceButton_Clicked;
            DeveloperProcessOptionsButton.Clicked += DeveloperProcessOptionsButton_Clicked;
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
