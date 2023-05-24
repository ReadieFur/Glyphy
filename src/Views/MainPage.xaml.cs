using Glyphy.Configuration;
using Glyphy.Controls;
using Glyphy.LED;
using Glyphy.Misc;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        if (Navigation.NavigationStack[Navigation.NavigationStack.Count - 1] is not IThemeChangeHandler themeChangeHandler)
            return;
        themeChangeHandler.RequestedThemeChanged(e.RequestedTheme == AppTheme.Dark);
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        foreach (string key in Enum.GetNames<EAddressable>())
            GlyphPreview.UpdatePreview(Enum.Parse<EAddressable>(key), 0);

        IEnumerable<Guid> animationIDs = Storage.GetAnimationIDs();
        foreach (Guid animationID in animationIDs)
        {
            if (await Storage.LoadAnimation(animationID) is not SAnimation sAnimation)
                continue;

            GlyphEntry glyphEntry = new() { ID = animationID };
            glyphEntry.Name = sAnimation.Name;
            //TODO: Clean this process up so it is more managable. (E.g. move these things to the class scope instead of a local scope).
            glyphEntry.OnActionButtonTapped += async (_, _) => await GlyphEntry_OnActionButtonTapped(glyphEntry);
            glyphEntry.OnEditButtonTapped += async (_, _) => await GlyphEntry_OnEditButtonTapped(glyphEntry);
            glyphEntry.OnDeleteButtonTapped += async (_, _) => await GlyphEntry_OnDeleteButtonTapped(glyphEntry);

            configurationsList.Add(glyphEntry);
        }

#if ANDROID
        Android_ContentPage_Loaded(sender, e);
#endif
    }

    private async Task GlyphEntry_OnActionButtonTapped(GlyphEntry glyphEntry)
    {
    }

    private async Task GlyphEntry_OnEditButtonTapped(GlyphEntry glyphEntry)
    {
        GlyphConfigurator glyphConfigurator = new(glyphEntry.ID);
        glyphConfigurator.Unloaded += (_, _) =>
        {
            glyphEntry.Name = glyphConfigurator.animation.Name;
            glyphConfigurator.Dispose();
        };
        await Navigation.PushAsync(glyphConfigurator);
    }

    private async Task GlyphEntry_OnDeleteButtonTapped(GlyphEntry glyphEntry)
    {
        Storage.DeleteAnimation(glyphEntry.ID);
        configurationsList.Remove(glyphEntry);
    }

    private async void NewConfigurationButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GlyphConfigurator());
    }
}
