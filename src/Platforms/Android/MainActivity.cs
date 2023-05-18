using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
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

        RefreshAPI();
        MainApplication.OnResume += _ => RefreshAPI();
    }

    private void SetStatusBarTheme(AppTheme requestedTheme)
    {
        if (Window is null)
            return;

        bool isDark = requestedTheme == AppTheme.Dark;

        Window.SetStatusBarColor(isDark ? Android.Graphics.Color.Black : Android.Graphics.Color.White);
#pragma warning disable CA1422 // Validate platform compatibility
        Window.DecorView.SystemUiVisibility = isDark ? (StatusBarVisibility)SystemUiFlags.Visible : (StatusBarVisibility)SystemUiFlags.LightStatusBar;
#pragma warning restore CA1422
    }

    private void RefreshAPI()
    {
        //We only have to start the API once, if superuser permissions are revoked mid-session, we can still use the API as that elevated process is still running.
        if (!LED.API.Running)
        {
            try { _ = LED.API.Instance; }
            catch (Exception ex)
            {
                if (ex is PermissionException
                    || ex is UnauthorizedAccessException)
                {
                    //Not sure about catching this last one for all calls.
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Superuser permissions required.", Android.Widget.ToastLength.Long)?.Show();
                }
                else throw;
            }
        }
    }
}
