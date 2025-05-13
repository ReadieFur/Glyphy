using Android.Content;
using Android.Provider;
using Glyphy.Storage;

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
                bool developerModeEnabled = Android.Provider.Settings.Global.GetInt(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Global.DevelopmentSettingsEnabled, 0) == 1;
                if (developerModeEnabled)
                    AboutDeviceSection.IsVisible = false;
            }
            catch {}

            AboutDeviceButton.Clicked += AboutDeviceButton_Clicked;
            DeveloperProcessOptionsButton.Clicked += DeveloperProcessOptionsButton_Clicked;
            TapGestureRecognizer tapGestureRecognizer = new();
            tapGestureRecognizer.Tapped += StoragePathLabel_Tapped;
            StoragePath.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void AboutDeviceButton_Clicked(object? sender, EventArgs e)
        {
            Intent intent = new(Android.Provider.Settings.ActionDeviceInfoSettings);
            //TODO: See if I can navigate directly to the "Software information" page.
            Platform.CurrentActivity?.StartActivity(intent);
        }

        private void DeveloperProcessOptionsButton_Clicked(object? sender, EventArgs e)
        {
            Intent intent = new(Android.Provider.Settings.ActionApplicationDevelopmentSettings);
            //TODO: See if I can have this scroll down to the "Background process limit" option.
            Platform.CurrentActivity?.StartActivity(intent);
        }

        private void StoragePathLabel_Tapped(object? sender, TappedEventArgs e)
        {
            if (!Directory.Exists(StorageManager.Instance.ExternalStoragePath))
                Directory.CreateDirectory(StorageManager.Instance.ExternalStoragePath);

            //https://stackoverflow.com/questions/17165972/android-how-to-open-a-specific-folder-via-intent-and-show-its-content-in-a-file
            Intent intent = new(Intent.ActionView);
            intent.SetDataAndType(Android.Net.Uri.Parse(StorageManager.Instance.ExternalStoragePath), "resource/folder");
            Platform.CurrentActivity?.StartActivity(intent);
        }
    }
}
