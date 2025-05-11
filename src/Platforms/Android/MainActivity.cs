using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Diagnostics;

namespace Glyphy;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    internal const bool AMBIENT_SERVICE_IS_FOREGROUND = false;

    internal static event Action<int, Result, Intent?>? OnActivityResultEvent;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        //https://stackoverflow.com/questions/73926834/net-maui-transparent-status-bar
        //TODO: Have a page wrapper pad the top and bottom of the page by the respective amounts for the status bar and navigation bar.
        Window!.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);
        Window!.ClearFlags(WindowManagerFlags.TranslucentStatus);
        Window!.SetStatusBarColor(Android.Graphics.Color.Transparent);

        

        Action<AppTheme> RequestedThemeChangedCallback = (appTheme) =>
        {
            if (Window is null)
                return;

            //https://stackoverflow.com/questions/72394044/how-to-make-status-bar-fully-transparent-in-net-maui-visual-studios-2022
            WindowInsetsControllerCompat windowInsetsController = new(Window, Window.DecorView);
            windowInsetsController.AppearanceLightStatusBars = appTheme == AppTheme.Light;
        };
        Microsoft.Maui.Controls.Application.Current!.RequestedThemeChanged += (_, args) => RequestedThemeChangedCallback(args.RequestedTheme);
        RequestedThemeChangedCallback(Microsoft.Maui.Controls.Application.Current!.RequestedTheme);
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        OnActivityResultEvent?.Invoke(requestCode, resultCode, data);
    }
}
