using Glyphy.LED;
using Glyphy.Misc;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;

namespace Glyphy.Views
{
    public partial class GlyphConfigurator : ContentPage, IThemeChangeHandler
    {
        private void Android_ContentPage_Loaded(object sender, EventArgs e)
        {
            RequestedThemeChanged(Application.Current!.RequestedTheme == AppTheme.Dark);

            //Potential race condition here where the check runs before the API starts.
            MainApplication.OnResume += MainApplication_OnResume;
        }

        private void MainApplication_OnResume(Android.App.Activity activity) =>
            ToggleControls(API.Running);

        public void RequestedThemeChanged(bool isDark)
        {
            MainActivity.Instance.SetSystemTheme(
                Android.Graphics.Color.ParseColor(isDark ? "#0D0D0D" : "#F2F2F2"),
                isDark ? Android.Graphics.Color.Black : Android.Graphics.Color.White,
                !isDark);
        }

        public void Android_Dispose()
        {
            MainApplication.OnResume -= MainApplication_OnResume;
        }
    }
}
