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
    //Temporary solution until I can access the Android Window object from my views.
    internal static MainActivity Instance { get; private set; } = null!;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        Instance = this;
    }

    public override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();

        RefreshAPI();
        MainApplication.OnResume += _ => RefreshAPI();
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
                    CommunityToolkit.Maui.Alerts.Toast.Make("Superuser permissions required.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
                }
                else throw;
            }
        }
    }

    public void SetSystemTheme(bool useDarkIcons)
    {
        if (Window is null)
            return;

#pragma warning disable CA1422 // Validate platform compatibility
        Window.DecorView.SystemUiVisibility = useDarkIcons ? (StatusBarVisibility)SystemUiFlags.LightStatusBar : (StatusBarVisibility)SystemUiFlags.Visible;
#pragma warning restore CA1422
    }
}
