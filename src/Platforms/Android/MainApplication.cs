using Android.App;
using Android.Runtime;
using Glyphy.Platforms.Android;
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

        return builder.Build();
    }
}
