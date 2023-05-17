using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace Glyphy.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
		VersionNumber.Text = AppInfo.VersionString;
	}
}
