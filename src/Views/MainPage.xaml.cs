namespace Glyphy.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

#if DEBUG
        private void DebugTestButton_Clicked(object sender, EventArgs e)
        {
        }
#endif

        private async void NotificationsButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NotificationsPage());
        }

        private async void SettingsButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private void NewConfigurationButton_Clicked(object sender, EventArgs e)
        {
        }
    }
}
