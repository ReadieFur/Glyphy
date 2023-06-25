using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Glyphy.Configuration;
using Glyphy.Platforms.Android;
using Glyphy.Platforms.Android.Services;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Glyphy;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    internal const bool AMBIENT_SERVICE_IS_FOREGROUND = false;

    internal static event Action<int, Result, Intent?>? OnActivityResultEvent;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        //Register services.
        //https://learn.microsoft.com/en-us/dotnet/api/android.content.pm.packagemanager.setcomponentenabledsetting?view=xamarin-android-sdk-13

        //Register services.
        Helpers.RegisterService<NotificationLightingService>();
        //Helpers.RegisterService<AmbientLightingService>(); //I don't think I need to do this for this service as the code I use within this method might only be relevant for "bound" services (i.e. services that are managed by the system).
        Helpers.RegisterService<AmbientLightingServiceQSTile>();

        //https://stackoverflow.com/questions/73926834/net-maui-transparent-status-bar
        //TODO: Have a page wrapper pad the top and bottom of the page by the respective amounts for the status bar and navigation bar.
        Window!.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);
        Window!.ClearFlags(WindowManagerFlags.TranslucentStatus);
        Window!.SetStatusBarColor(Android.Graphics.Color.Transparent);

#pragma warning disable CA1422 // Validate platform compatibility
        Action<AppTheme> RequestedThemeChangedCallback = appTheme => Window!.DecorView.SystemUiVisibility = appTheme == AppTheme.Light ? (StatusBarVisibility)SystemUiFlags.LightStatusBar : (StatusBarVisibility)SystemUiFlags.Visible;
        Microsoft.Maui.Controls.Application.Current!.RequestedThemeChanged += (_, args) => RequestedThemeChangedCallback(args.RequestedTheme);
        RequestedThemeChangedCallback(Microsoft.Maui.Controls.Application.Current!.RequestedTheme);
#pragma warning restore CA1422
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        OnActivityResultEvent?.Invoke(requestCode, resultCode, data);
    }

    public override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();

        RefreshAPI();
    }

    protected override void OnResume()
    {
        base.OnResume();
        
        RefreshAPI();

        Helpers.StopService<AmbientLightingService>(AMBIENT_SERVICE_IS_FOREGROUND);
    }

    protected override void OnPause()
    {
        base.OnPause();

        Task.Run(async () =>
        {
            if ((await Storage.AmbientService.GetCached()).Enabled)
                Helpers.StartService<AmbientLightingService>(AMBIENT_SERVICE_IS_FOREGROUND);
        });
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
}
