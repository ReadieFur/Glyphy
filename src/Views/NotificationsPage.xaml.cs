using Glyphy.Storage;

namespace Glyphy.Views;

public partial class NotificationsPage : ContentPage
{
    private readonly NotificationsPageViewModel _viewModel = new();

    public NotificationsPage()
	{
		InitializeComponent();

        BindingContext = _viewModel;
        _viewModel.Enabled = StorageManager.Instance.Settings.NotificationServiceEnabled;

#if ANDROID
        Android_Constructor();
#endif
    }

    private async void BackButton_Clicked(object sender, System.EventArgs e)
    {
        StorageManager.Instance.Settings.NotificationServiceEnabled = _viewModel.Enabled;
        await StorageManager.Instance.SaveSettings();

        await Navigation.PopAsync();
    }
}
