using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui;
using ForegroundService = Glyphy.Platforms.Android.ForegroundService;

namespace Glyphy;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        //Start service
        var serviceIntent = new Intent(this, typeof(ForegroundService));
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            NotificationChannel channel = new NotificationChannel("glyphy", "Glyphy", NotificationImportance.Min);
            channel.SetSound(null, null);
            channel.EnableVibration(false);
            //channel.LockscreenVisibility = NotificationVisibility.Secret;
            NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);

            StartForegroundService(serviceIntent);
        }
        else
            StartService(serviceIntent);
    }
}
