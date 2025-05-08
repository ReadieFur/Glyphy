// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using Microsoft.UI.Windowing;
using Microsoft.UI;
using Windows.Graphics;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using System;

namespace Glyphy.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();

        //It seems I forgot to link the source to this...
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {
            var mauiWindow = handler.VirtualView;
            var nativeWindow = handler.PlatformView;
            nativeWindow.Activate();

            //Allow Windows to draw a native titlebar which respects IsMaximizable/IsMinimizable
            nativeWindow.ExtendsContentIntoTitleBar = false;

            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

            //Set a specific window size.
            float aspectRatio = 1080f / 2400f;
            int desiredWidth = 540;
            appWindow.Resize(new SizeInt32(desiredWidth, (int)(desiredWidth / aspectRatio)));

            if (appWindow.Presenter is OverlappedPresenter p)
            {
                p.IsResizable = false;

                //These only have effect if XAML isn't responsible for drawing the titlebar.
                p.IsMaximizable = false;
                p.IsMinimizable = false;
            }
        });
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateBuilder().Build();
}
