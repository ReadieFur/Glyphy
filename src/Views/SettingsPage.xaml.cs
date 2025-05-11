using System.ComponentModel;

namespace Glyphy.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            BindingContext = new SettingsPageViewModel();

#if ANDROID
            Android_Constructor();
#endif
        }

        private void ContentPage_Loaded(object sender, EventArgs e)
        {
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
