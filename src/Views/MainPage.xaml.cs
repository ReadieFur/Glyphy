using Glyphy.Animation;
using Glyphy.Configuration;
using Glyphy.Controls;
using Glyphy.LED;
using Glyphy.Misc;
using Glyphy.Resources.Presets;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers = Glyphy.Misc.Helpers;

namespace Glyphy.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
	{
		InitializeComponent();
    }

#if DEBUG
    private void DebugTestButton_Clicked(object sender, EventArgs e)
    {
       API.Instance.DebugTest();
    }
#endif

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        foreach (EAddressable value in Enum.GetValues<EAddressable>())
            GlyphPreview.UpdatePreview(value, 0);

        //TODO: This can have a cleaner solution other than Task.Run right?
        Task.Run(async () =>
        {
            bool failedToLoad = false;

            //TODO: While we are loading these animations, disable controls and show a loading animation.
            foreach (Guid animationID in Storage.GetAnimationIDs())
            {
                GlyphEntry glyphEntry;

                try
                {
                    if (await Storage.LoadAnimation(animationID) is not SAnimation sAnimation)
                        throw new Exception("Failed to load animation from storage.");

                    glyphEntry = new(
                        animationID,
                        Glyphs.Presets.Any(preset => preset.Id == animationID) //If the animation is a preset, make it readonly (this is slow as I check for the the ID every time, more presets = more time).
                    );
                }
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

        //Assume the app is focused when this is called (it should be).
        AnimationRunner.OnRunFrame += AnimationRunner_OnRunFrame;

#if ANDROID
        Android_ContentPage_Loaded(sender, e);
#endif
    }

    private void AnimationRunner_OnRunFrame(IReadOnlyList<SLEDValue> ledValues)
    {
        foreach (SLEDValue ledValue in ledValues)
            GlyphPreview.UpdatePreview(ledValue.Led, ledValue.Brightness);
    }

    private async void NewConfigurationButton_Clicked(object sender, EventArgs e)
    {
        GlyphConfigurator glyphConfigurator = new();
        glyphConfigurator.Disappearing += (_, _) => Task.Run(() =>
        {
            if (!Storage.GetAnimationIDs().Contains(glyphConfigurator.Animation.Id))
                return;

            GlyphEntry glyphEntry;
            try { glyphEntry = new(glyphConfigurator.Animation.Id); }
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

    private async void NotificationsButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NotificationsPage());
    }

    private async void SettingsButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage());
    }

    private void Android_ContentPage_Loaded(object sender, System.EventArgs e)
    {
        Padding = new(Padding.Left, /*Padding.Top + */Helpers.StatusBarHeight, Padding.Right, /*Padding.Bottom + */Helpers.NavigationBarHeight);

        Platform.ActivityStateChanged += Platform_ActivityStateChanged;
    }

    private void Platform_ActivityStateChanged(object? sender, ActivityStateChangedEventArgs e)
    {
        //To save CPU time, disable the Animation.AnimationRunner.OnRunFrame callback when the app isn't focused.
        switch (e.State)
        {
            case ActivityState.Started:
            case ActivityState.Resumed:
                Animation.AnimationRunner.OnRunFrame += AnimationRunner_OnRunFrame;
                break;
            case ActivityState.Stopped:
            case ActivityState.Paused:
                Animation.AnimationRunner.OnRunFrame -= AnimationRunner_OnRunFrame;
                break;
            default:
                break;
        }
    }
}
