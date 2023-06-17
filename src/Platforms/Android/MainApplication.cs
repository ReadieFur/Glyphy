using Android.App;
using Android.Runtime;
using Glyphy.Platforms.Android;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using System;
using System.Diagnostics;

namespace Glyphy;

[Application]
public class MainApplication : MauiApplication
{
    public static event AndroidLifecycle.OnResume? OnResume;

    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

    protected override MauiApp CreateMauiApp()
	{
		MauiAppBuilder builder = MauiProgram.CreateBuilder();
        builder
            .ConfigureLifecycleEvents(events =>
            {
                events.AddAndroid(android => android
                    .OnCreate((activity, _) =>
                    {
                        //https://stackoverflow.com/questions/73926834/net-maui-transparent-status-bar
                        //TODO: Have the page wrapper pad the top and bottom of the page by the respective amounts for the status bar and navigation bar.
                        //activity.Window?.SetFlags(Android.Views.WindowManagerFlags.LayoutNoLimits, Android.Views.WindowManagerFlags.LayoutNoLimits);
                        activity.Window?.ClearFlags(Android.Views.WindowManagerFlags.TranslucentStatus);
                        activity.Window?.SetStatusBarColor(Android.Graphics.Color.Transparent);
                    })
                    .OnResume(e => OnResume?.Invoke(e))
                );
            });

        //It turns out the line I want to remove is the autocorrect line, not the underline line so I have removed the following for now.
        //https://github.com/dotnet/maui/issues/7906#issuecomment-1264717945
        /*Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
            h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent));*/

        return builder.Build();
    }
}
