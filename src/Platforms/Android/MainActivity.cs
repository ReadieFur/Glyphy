using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;

namespace Glyphy;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        
        SetStatusBarTheme(Microsoft.Maui.Controls.Application.Current!.RequestedTheme);
        Microsoft.Maui.Controls.Application.Current!.RequestedThemeChanged += (_, e) => SetStatusBarTheme(e.RequestedTheme);

        MainApplication.OnResume += MainApplication_OnResume;
        //Validate root access.
    }

    private void SetStatusBarTheme(AppTheme requestedTheme)
    {
        bool isDark = requestedTheme == AppTheme.Dark;
        Window.SetStatusBarColor(isDark ? Android.Graphics.Color.Black : Android.Graphics.Color.White);
        Window.DecorView.SystemUiVisibility = isDark ? (StatusBarVisibility)SystemUiFlags.Visible : (StatusBarVisibility)SystemUiFlags.LightStatusBar;
    }

    private void MainApplication_OnResume(Activity activity)
    {
        //Revalidate root access.
    }
}
