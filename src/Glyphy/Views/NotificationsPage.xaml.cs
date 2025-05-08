using Microsoft.Maui.Controls;

namespace Glyphy.Views;

public partial class NotificationsPage : ContentPage
{
	public NotificationsPage()
	{
		InitializeComponent();
	}

    private void ContentPage_Loaded(object sender, System.EventArgs e)
    {
#if ANDROID
        Android_ContentPage_Loaded(sender, e);
#endif
    }

    private void BackButton_Clicked(object sender, System.EventArgs e)
    {
        Navigation.PopAsync();
    }
}
