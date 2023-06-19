using Glyphy.LED;
using Glyphy.Platforms.Android;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;

namespace Glyphy.Views
{
    public partial class GlyphConfigurator : ContentPage
    {
        private void Android_ContentPage_Loaded(object sender, EventArgs e)
        {
            Header.Padding = new(Header.Padding.Left, Header.Padding.Top + Helpers.StatusBarHeight, Header.Padding.Right, Header.Padding.Bottom);
            Padding = new(Padding.Left, Padding.Top, Padding.Right, Padding.Bottom + Helpers.NavigationBarHeight);

            //Potential race condition here where the check runs before the API starts.
            MainApplication.OnResume += MainApplication_OnResume;
        }

        private void MainApplication_OnResume(Android.App.Activity activity) =>
            ToggleControls(API.Running);

        public void Android_Dispose()
        {
            MainApplication.OnResume -= MainApplication_OnResume;
        }
    }
}
