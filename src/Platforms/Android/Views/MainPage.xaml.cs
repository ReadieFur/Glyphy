using Glyphy.LED;
using Microsoft.Maui.Controls;
using System;

namespace Glyphy.Views
{
    public partial class MainPage : ContentPage
    {
        private void Android_ContentPage_Loaded(object sender, EventArgs e)
        {
            //Potential race condition here where the check runs before the API starts.
            MainApplication.OnResume += _ => ToggleControls(API.Running);
        }
    }
}
