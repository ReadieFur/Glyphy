using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace Glyphy;

public static class MauiProgram
{
	public static MauiAppBuilder CreateBuilder()
	{
        MauiAppBuilder builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("ndot.otf", "dotmatrix");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder;
	}
}
