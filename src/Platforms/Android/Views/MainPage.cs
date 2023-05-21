using Microsoft.Maui.Controls;

namespace Glyphy.Views
{
    public partial class MainPage : ContentPage, IThemeChangeHandler
    {
        private void Android_ContentPage_Loaded(object sender, System.EventArgs e)
        {
            RequestedThemeChanged(Application.Current!.RequestedTheme == Microsoft.Maui.ApplicationModel.AppTheme.Dark);
        }

        public void RequestedThemeChanged(bool isDark)
        {
            MainActivity.Instance.SetSystemTheme(
                Android.Graphics.Color.ParseColor(isDark ? "#000000" : "#FFFFFF"),
                isDark ? Android.Graphics.Color.Black : Android.Graphics.Color.White,
                !isDark);
        }
    }
}
