using Java.Lang;
using Microsoft.Maui.Controls;
using System;

namespace Glyphy
{
    public partial class MainPage : ContentPage
    {
        private void ContentPage_Loaded(object sender, EventArgs e)
        {
            MainApplication.OnResume += MainApplication_OnResume;

            //Revalidate root access.
        }

        private void MainApplication_OnResume(Android.App.Activity activity)
        {
            //Revalidate root access.
        }
    }
}
