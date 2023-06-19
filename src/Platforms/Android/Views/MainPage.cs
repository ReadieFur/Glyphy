using Glyphy.Platforms.Android;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace Glyphy.Views
{
    public partial class MainPage : ContentPage
    {
        private void Android_ContentPage_Loaded(object sender, System.EventArgs e)
        {
            Padding = new(Padding.Left, Padding.Top + Helpers.StatusBarHeight, Padding.Right, Padding.Bottom + Helpers.NavigationBarHeight);
        }
    }
}
