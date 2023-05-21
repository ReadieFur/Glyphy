using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Threading;

namespace Glyphy.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
	{
		InitializeComponent();
		VersionNumber.Text = AppInfo.VersionString;
	}

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        //TESTING ONLY
        Navigation.PushModalAsync(new GlyphConfigurator());
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new GlyphConfigurator());
    }
}
