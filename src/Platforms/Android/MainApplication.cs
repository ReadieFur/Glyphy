using Android.App;
using Android.Runtime;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using System;

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
                    .OnResume(e => OnResume?.Invoke(e))
                );
            });



        //It turns out the line I want to remove is the auto-correct line, not the underline line so I have removed the following for now.
        //https://github.com/dotnet/maui/issues/7906#issuecomment-1264717945
        /*Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
            h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent));*/

        return builder.Build();
    }
}
