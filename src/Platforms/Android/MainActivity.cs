using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.App;
using Glyphy.Platforms.Android;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Glyphy;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    //Temporary solution until I can access the Android Window object from my views.
    internal static MainActivity Instance { get; private set; } = null!;

    private event Action<int, Result, Intent?>? OnActivityResultEvent;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        if (Instance is null)
            Instance = this;

        base.OnCreate(savedInstanceState);

        //Register services.
        //https://learn.microsoft.com/en-us/dotnet/api/android.content.pm.packagemanager.setcomponentenabledsetting?view=xamarin-android-sdk-13

        PackageManager!.SetComponentEnabledSetting(
            new(this, Java.Lang.Class.FromType(typeof(NotificationListenerService))),
            ComponentEnabledState.Enabled,
            ComponentEnableOption.DontKillApp);

        //TODO: Move this request to a UI element.
        //Task.Run(RequestNotificationAccess);

        //https://stackoverflow.com/questions/73926834/net-maui-transparent-status-bar
        //TODO: Have a page wrapper pad the top and bottom of the page by the respective amounts for the status bar and navigation bar.
        Window!.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);
        Window!.ClearFlags(WindowManagerFlags.TranslucentStatus);
        Window!.SetStatusBarColor(Android.Graphics.Color.Transparent);

        Microsoft.Maui.Controls.Application.Current!.RequestedThemeChanged += (_, args) =>
        {
#pragma warning disable CA1422 // Validate platform compatibility
            Window!.DecorView.SystemUiVisibility = args.RequestedTheme == AppTheme.Light ? (StatusBarVisibility)SystemUiFlags.LightStatusBar : (StatusBarVisibility)SystemUiFlags.Visible;
#pragma warning restore CA1422
        };

#pragma warning disable CA1422 // Validate platform compatibility
        Window!.DecorView.SystemUiVisibility = Microsoft.Maui.Controls.Application.Current!.RequestedTheme == AppTheme.Light ? (StatusBarVisibility)SystemUiFlags.LightStatusBar : (StatusBarVisibility)SystemUiFlags.Visible;
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

    public async Task<bool> RequestNotificationAccess() => await RequestNotificationAccess(null);

    public async Task<bool> RequestNotificationAccess(CancellationToken? cancellationToken = null)
    {
        const int NOTIFICATION_ACCESS_REQUEST_CODE = 1001;

        if (PackageName is null)
            throw new NullReferenceException(nameof(PackageName));

        if (NotificationManagerCompat.GetEnabledListenerPackages(this).Contains(PackageName))
            return true;

        ManualResetEventSlim activityResultResetEvent = new(false);

        Intent intent = new("android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS");
        StartActivityForResult(intent, NOTIFICATION_ACCESS_REQUEST_CODE);

        OnActivityResultEvent += (requestCode, resultCode, data) =>
        {
            if (requestCode == NOTIFICATION_ACCESS_REQUEST_CODE)
                activityResultResetEvent.Set();
        };

        await Task.Run(() =>
        {
            if (cancellationToken is null)
                activityResultResetEvent.Wait();
            else
                activityResultResetEvent.Wait(cancellationToken.Value);
        });

        return NotificationManagerCompat.GetEnabledListenerPackages(this).Contains(PackageName);
    }
}
