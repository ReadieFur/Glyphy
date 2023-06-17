using Glyphy.Misc;
using Microsoft.Maui.Controls;

namespace Glyphy.Views
{
    public partial class MainPage : ContentPage, IThemeChangeHandler
    {
        private void Android_ContentPage_Loaded(object sender, System.EventArgs e)
        {
            RequestedThemeChanged(Application.Current!.RequestedTheme == Microsoft.Maui.ApplicationModel.AppTheme.Dark);
        }

        public void RequestedThemeChanged(bool isDark) => MainActivity.Instance.SetSystemTheme(!isDark);
    }
}
