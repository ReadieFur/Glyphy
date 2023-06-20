using Glyphy.Platforms.Android;
using System;

namespace Glyphy.Views
{
    public partial class SettingsPage
    {
        private void Android_ContentPage_Loaded(object sender, EventArgs e)
        {
            Header.Padding = new(Header.Padding.Left, /*Header.Padding.Top + */Helpers.StatusBarHeight, Header.Padding.Right, Header.Padding.Bottom);
            Padding = new(Padding.Left, Padding.Top, Padding.Right, /*Padding.Bottom + */Helpers.NavigationBarHeight);
        }
    }
}
