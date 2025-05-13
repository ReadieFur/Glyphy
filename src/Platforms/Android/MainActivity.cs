using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using Window = Android.Views.Window;
using MauiColor = Microsoft.Maui.Graphics.Color;
using AndroidColor = Android.Graphics.Color;
using Glyphy.Platforms.Android;
using Glyphy.Platforms.Android.Services;
using Glyphy.Storage;

namespace Glyphy
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        internal const bool GLYPH_SERVICES_ARE_FOREGROUND = false;

        internal delegate void ActivityResultEventHandler(int requestCode, Result resultCode, Intent? data);

        internal static event ActivityResultEventHandler? OnActivityResultEvent;

        internal static void SetSystemUIScheme(AndroidColor statusBarColour, AndroidColor navigationBarColour, bool statusBarLight, bool navigationBarLight)
        {
            if (Platform.CurrentActivity?.Window is not Window window)
                return;

            //https://stackoverflow.com/questions/72394044/how-to-make-status-bar-fully-transparent-in-net-maui-visual-studios-2022
            WindowInsetsControllerCompat windowInsetsController = WindowCompat.GetInsetsController(window, window.DecorView);

            window.SetStatusBarColor(statusBarColour);
            window.SetNavigationBarColor(navigationBarColour);
            windowInsetsController.AppearanceLightStatusBars = !statusBarLight;
            windowInsetsController.AppearanceLightNavigationBars = !navigationBarLight;
        }

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //Register services.
            AndroidHelpers.RegisterService<NotificationLightingService>();
            /** AmbientLightingService remark:
             * I don't think I need to do this for this service as the code I use within this method might only be relevant for "bound" services
             * i.e. services that are managed by the system.
             */
            //AndroidHelpers.RegisterService<AmbientLightingService>(); 

            if (Window is null)
                return;

            //Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);
            Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            Window.ClearFlags(WindowManagerFlags.TranslucentNavigation);

            bool isDarkTheme = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;
            if (Microsoft.Maui.Controls.Application.Current?.Resources[isDarkTheme ? "BackgroundDark" : "Background"] is MauiColor mauiColour)
            {
                mauiColour.ToRgb(out byte r, out byte g, out byte b);
                AndroidColor androidColor = new(r, g, b);
                SetSystemUIScheme(androidColor, androidColor, isDarkTheme, isDarkTheme);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            OnActivityResultEvent?.Invoke(requestCode, resultCode, data);
        }

        //This is called just after the app goes out of focus and so if the background process limit is set to 0, I have found this doesn't get called.
        protected override void OnPause()
        {
            base.OnPause();

            if (StorageManager.Instance.Settings.AmbientServiceEnabled)
                AndroidHelpers.StartService<AmbientLightingService>(GLYPH_SERVICES_ARE_FOREGROUND);
        }

        protected override void OnResume()
        {
            base.OnResume();

            //Possibly refresh the system UI theme here? (Although better to do from the UI context).

            if (AndroidHelpers.IsServiceRunning<AmbientLightingService>())
                AndroidHelpers.StopService<AmbientLightingService>(GLYPH_SERVICES_ARE_FOREGROUND);
        }
    }
}
