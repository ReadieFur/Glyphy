using Glyphy.Storage;

namespace Glyphy.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            BindingContext = new SettingsPageViewModel();

            StoragePath.Text += StorageManager.Instance.ExternalStoragePath;

#if ANDROID
            Android_Constructor();
#endif
        }

        private async void BackButton_Clicked(object? sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
