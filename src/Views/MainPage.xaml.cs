using Glyphy.Configuration;
using Glyphy.Controls;
using Glyphy.LED;
using Glyphy.Misc;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Glyphy.Views;

public partial class MainPage : ContentPage, IDisposable
{
    public MainPage()
	{
		InitializeComponent();

        VersionNumber.Text = AppInfo.VersionString;

        Application.Current!.RequestedThemeChanged += MainPage_RequestedThemeChanged;
    }

    public void Dispose()
    {
        Application.Current!.RequestedThemeChanged -= MainPage_RequestedThemeChanged;
    }

    private void MainPage_RequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        if (Navigation.NavigationStack.Count != 0 && Navigation.NavigationStack[Navigation.NavigationStack.Count - 1] is IThemeChangeHandler themeChangeHandler)
            themeChangeHandler.RequestedThemeChanged(e.RequestedTheme == AppTheme.Dark);
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        foreach (string key in Enum.GetNames<EAddressable>())
            GlyphPreview.UpdatePreview(Enum.Parse<EAddressable>(key), 0);

        //TODO: This can have a cleaner solution other than Task.Run right?
        Task.Run(async () =>
        {
            bool failedToLoad = false;

            //TODO: While we are loading these animations, disable controls and show a loading animation.
            IEnumerable<Guid> animationIDs = Storage.GetAnimationIDs();
            foreach (Guid animationID in animationIDs)
            {
                if (await Storage.LoadAnimation(animationID) is not SAnimation sAnimation)
                    continue;

                GlyphEntry glyphEntry;
                try { glyphEntry = new(animationID); }
                catch
                {
                    failedToLoad = true;
                    continue;
                }

                glyphEntry.OnDeleted += (_, _) => configurationsList.Remove(glyphEntry);

                Dispatcher.Dispatch(() => configurationsList.Add(glyphEntry));
            }

            if (failedToLoad)
                _ = CommunityToolkit.Maui.Alerts.Toast.Make("Failed to load one or more animations.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
        });

#if ANDROID
        Android_ContentPage_Loaded(sender, e);
#endif
    }

    private async void NewConfigurationButton_Clicked(object sender, EventArgs e)
    {
        GlyphConfigurator glyphConfigurator = new();
        glyphConfigurator.Disappearing += (_, _) => Task.Run(() =>
        {
            if (Navigation.NavigationStack.Count != 0 && Navigation.NavigationStack[Navigation.NavigationStack.Count - 1] is IThemeChangeHandler themeChangeHandler)
                themeChangeHandler.RequestedThemeChanged(Application.Current!.RequestedTheme == AppTheme.Dark);

            if (!Storage.GetAnimationIDs().Contains(glyphConfigurator.animation.Id))
                return;

            GlyphEntry glyphEntry;
            try { glyphEntry = new(glyphConfigurator.animation.Id); }
            catch
            {
                _ = CommunityToolkit.Maui.Alerts.Toast.Make("Failed to update animations.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
                return;
            }

            glyphEntry.OnDeleted += (_, _) => configurationsList.Remove(glyphEntry);

            Dispatcher.Dispatch(() => configurationsList.Add(glyphEntry));
        });
        await Navigation.PushAsync(glyphConfigurator, true);
    }
}
