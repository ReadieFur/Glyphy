namespace Glyphy.Views;

public partial class NotificationsPage : ContentPage
{
	public NotificationsPage()
	{
		InitializeComponent();

        BindingContext = new NotificationsPageViewModel();

#if ANDROID
        Android_Constructor();
#endif
    }

    private async void BackButton_Clicked(object sender, System.EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
