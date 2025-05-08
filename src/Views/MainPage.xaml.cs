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
    }

    private void AnimationRunner_OnRunFrame(IReadOnlyList<SLEDValue> ledValues)
    {
    }

    private async void NewConfigurationButton_Clicked(object sender, EventArgs e)
    {
    }

    private async void NotificationsButton_Clicked(object sender, EventArgs e)
    {
    }

    private async void SettingsButton_Clicked(object sender, EventArgs e)
    {
    }

    private void Platform_ActivityStateChanged(object? sender, ActivityStateChangedEventArgs e)
    {
    }
}
