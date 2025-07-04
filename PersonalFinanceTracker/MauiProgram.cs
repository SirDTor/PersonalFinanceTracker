﻿using Microcharts.Maui;
using Microsoft.Extensions.Logging;

namespace PersonalFinanceTracker;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.UseMicrocharts();			

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
