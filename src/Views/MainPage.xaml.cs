using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;

namespace Glyphy.Views;

public partial class MainPage : ContentPage, IDisposable
{
    public MainPage()
	{
		InitializeComponent();

        Application.Current!.RequestedThemeChanged += MainPage_RequestedThemeChanged;

        VersionNumber.Text = AppInfo.VersionString;
    }

    public void Dispose()
    {
        Application.Current!.RequestedThemeChanged -= MainPage_RequestedThemeChanged;
    }

    private void MainPage_RequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        if (Navigation.NavigationStack.Count == 0)
            return;

        IThemeChangeHandler? themeChangeHandler = Navigation.NavigationStack[Navigation.NavigationStack.Count - 1] as IThemeChangeHandler;
        if (themeChangeHandler is null)
            return;

        themeChangeHandler.RequestedThemeChanged(e.RequestedTheme == AppTheme.Dark);
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
#if ANDROID
        Android_ContentPage_Loaded(sender, e);
#endif

        //TESTING ONLY
        //Navigation.PushAsync(new GlyphConfigurator());
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GlyphConfigurator());
    }
}
