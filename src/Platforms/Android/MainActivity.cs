using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using Window = Android.Views.Window;
using MauiColor = Microsoft.Maui.Graphics.Color;
using AndroidColor = Android.Graphics.Color;

namespace Glyphy
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
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
    }
}
