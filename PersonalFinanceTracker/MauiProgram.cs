using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using PersonalFinanceTracker.Services;
using PersonalFinanceTracker.ViewModels;
using PersonalFinanceTracker.Views;

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
				fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
			})
			.UseMicrocharts();

// Register Services
builder.Services.AddSingleton<IDatabaseService>(provider =>
	new DatabaseService(Path.Combine(FileSystem.AppDataDirectory, "financedb.db")));
builder.Services.AddTransient<IAnalyticsService, AnalyticsService>();

// Register ViewModels
builder.Services.AddSingleton<MainViewModel>();
builder.Services.AddTransient<AddTransactionViewModel>();
builder.Services.AddTransient<EditTransactionViewModel>();
builder.Services.AddTransient<TransactionListViewModel>();
builder.Services.AddTransient<GoalViewModel>();
builder.Services.AddTransient<BudgetViewModel>();
builder.Services.AddTransient<AnalyticsViewModel>();
builder.Services.AddTransient<SettingsViewModel>();
builder.Services.AddTransient<ExportViewModel>();
builder.Services.AddTransient<PinSettingsViewModel>();
builder.Services.AddTransient<PinUnlockViewModel>();

		// Register Views
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddTransient<AddTransactionPage>();
		builder.Services.AddTransient<EditTransactionPage>();
		builder.Services.AddTransient<TransactionListPage>();
		builder.Services.AddTransient<GoalsPage>();
		builder.Services.AddTransient<AddGoalPage>();
		builder.Services.AddTransient<SettingsPage>();
		builder.Services.AddTransient<ExportPage>();
		builder.Services.AddTransient<BudgetsPage>();
		builder.Services.AddTransient<PinSettingsPage>();
		builder.Services.AddTransient<PinUnlockPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
