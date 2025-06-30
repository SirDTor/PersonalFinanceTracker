using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;

namespace PersonalFinanceTracker.ViewModels;

public class AnalyticsViewModel : BaseViewModel
{
    private readonly IAnalyticsService _analyticsService;

    private SpendingAnalytics? _spendingAnalytics;
    public SpendingAnalytics? SpendingAnalytics
    {
        get => _spendingAnalytics;
        set
        {
            _spendingAnalytics = value;
            OnPropertyChanged();
            // Уведомляем зависимые свойства для UI
            OnPropertyChanged(nameof(TotalSpentText));
            OnPropertyChanged(nameof(TotalIncomeText));
            OnPropertyChanged(nameof(NetAmountText));
            OnPropertyChanged(nameof(DailyAverageText));
            OnPropertyChanged(nameof(NetAmountColor));
            OnPropertyChanged(nameof(HasPositiveNet));
            OnPropertyChanged(nameof(HasNegativeNet));
        }
    }

    public ObservableCollection<CategorySpending> CategorySpending { get; set; } = new();
    public ObservableCollection<MonthlyTrend> MonthlyTrends { get; set; } = new();
    public ObservableCollection<FinancialGoalProgress> GoalProgress { get; set; } = new();

    public string TotalSpentText => SpendingAnalytics?.TotalExpenses.ToString("C") ?? "$0.00";
    public string TotalIncomeText => SpendingAnalytics?.TotalIncome.ToString("C") ?? "$0.00";
    public string NetAmountText => SpendingAnalytics?.NetAmount.ToString("C") ?? "$0.00";
    public string DailyAverageText => SpendingAnalytics?.AverageDailySpending.ToString("C") ?? "$0.00";

    public string NetAmountColor => SpendingAnalytics is { NetAmount: > 0 } ? "#20BF6B" : "#EE5A24";
    public bool HasPositiveNet => SpendingAnalytics is { NetAmount: > 0 };
    public bool HasNegativeNet => SpendingAnalytics is { NetAmount: <= 0 };

    public AnalyticsViewModel(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    public async Task LoadAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        IsBusy = true;
        try
        {
            var analyticsTask = _analyticsService.GetSpendingAnalyticsAsync(startDate, endDate);
            var categoryTask = _analyticsService.GetCategoryBreakdownAsync(startDate, endDate);
            var trendsTask = _analyticsService.GetMonthlyTrendsAsync(12);
            var goalsTask = _analyticsService.GetGoalProgressAsync();

            await Task.WhenAll(analyticsTask, categoryTask, trendsTask, goalsTask);

            SpendingAnalytics = analyticsTask.Result;

            CategorySpending.Clear();
            foreach (var item in categoryTask.Result)
                CategorySpending.Add(item);

            MonthlyTrends.Clear();
            foreach (var item in trendsTask.Result)
                MonthlyTrends.Add(item);

            GoalProgress.Clear();
            foreach (var item in goalsTask.Result)
                GoalProgress.Add(item);
        }
        catch (Exception ex)
        {
            // Тут можно залогировать ошибку
            Console.WriteLine($"Error loading analytics: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<decimal> GetMonthlyIncomeDifferenceAsync(DateTime month1, DateTime month2)
    {
        try
        {
            var result = await _analyticsService.GetMonthlyComparisonAsync(month1, month2);
            return result.GetValueOrDefault("CurrentIncome") - result.GetValueOrDefault("PreviousIncome");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error comparing monthly income: {ex.Message}");
            return 0;
        }
    }

    public async Task<decimal> GetMonthlyExpenseDifferenceAsync(DateTime month1, DateTime month2)
    {
        try
        {
            var result = await _analyticsService.GetMonthlyComparisonAsync(month1, month2);
            return result.GetValueOrDefault("CurrentExpenses") - result.GetValueOrDefault("PreviousExpenses");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error comparing monthly expenses: {ex.Message}");
            return 0;
        }
    }

    public async Task<List<string>> GetSpendingInsightsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            return await _analyticsService.GetSpendingInsightsAsync(startDate, endDate);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting spending insights: {ex.Message}");
            return new List<string>();
        }
    }
}
