using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Diagnostics;

namespace Glyphy;

[Activity(Theme = "@style/AndroidAppTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (Window is null)
            throw new NullReferenceException(nameof(Window));

        //https://stackoverflow.com/questions/73926834/net-maui-transparent-status-bar
        //TODO: Have a page wrapper pad the top and bottom of the page by the respective amounts for the status bar and navigation bar.
        //Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);
        Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
        Window.SetStatusBarColor(Android.Graphics.Color.Transparent);

        /*if (Microsoft.Maui.Controls.Application.Current is null)
            throw new NullReferenceException(nameof(Microsoft.Maui.Controls.Application.Current));

        Action<AppTheme> RequestedThemeChangedCallback = (appTheme) =>
        {
            if (Window is null)
                return;

            //https://stackoverflow.com/questions/72394044/how-to-make-status-bar-fully-transparent-in-net-maui-visual-studios-2022
            WindowCompat.GetInsetsController(Window, Window.DecorView).AppearanceLightStatusBars = appTheme == AppTheme.Light;
        };
        Microsoft.Maui.Controls.Application.Current.RequestedThemeChanged += (_, args) => RequestedThemeChangedCallback(args.RequestedTheme);
        RequestedThemeChangedCallback(Microsoft.Maui.Controls.Application.Current.RequestedTheme);*/
    }
}
