using Glyphy.Platforms.Android;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace Glyphy.Views
{
    public partial class MainPage : ContentPage
    {
        private void Android_ContentPage_Loaded(object sender, System.EventArgs e)
        {
            Padding = new(Padding.Left, /*Padding.Top + */Helpers.StatusBarHeight, Padding.Right, /*Padding.Bottom + */Helpers.NavigationBarHeight);

            Platform.ActivityStateChanged += Platform_ActivityStateChanged;
        }

        private void Platform_ActivityStateChanged(object? sender, ActivityStateChangedEventArgs e)
        {
            //To save CPU time, disable the Animation.AnimationRunner.OnRunFrame callback when the app isn't focused.
            switch (e.State)
            {
                case ActivityState.Started:
                case ActivityState.Resumed:
                    Animation.AnimationRunner.OnRunFrame += AnimationRunner_OnRunFrame;
                    break;
                case ActivityState.Stopped:
                case ActivityState.Paused:
                    Animation.AnimationRunner.OnRunFrame -= AnimationRunner_OnRunFrame;
                    break;
                default:
                    break;
            }
        }
    }
}
