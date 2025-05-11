using MauiColor = Microsoft.Maui.Graphics.Color;
using AndroidColor = Android.Graphics.Color;

namespace Glyphy
{
    public partial class App : Application
    {
        private void Current_RequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
        {
            bool isDarkTheme = e.RequestedTheme == AppTheme.Dark;
            if (Current?.Resources[isDarkTheme ? "BackgroundDark" : "Background"] is not MauiColor mauiColour)
                return;
            mauiColour.ToRgb(out byte r, out byte g, out byte b);
            AndroidColor androidColor = new(r, g, b);
            MainActivity.SetSystemUIScheme(androidColor, androidColor, isDarkTheme, isDarkTheme);
        }
    }
}
