using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace PersonalFinanceTracker.Models;

public partial class SpendingAnalytics : ObservableObject
{
    [ObservableProperty]
    private decimal totalIncome;

    [ObservableProperty]
    private decimal totalExpenses;

    [ObservableProperty]
    private decimal totalSpent;

    [ObservableProperty]
    private decimal netAmount;

    [ObservableProperty]
    private decimal savingsRate;

    [ObservableProperty]
    private decimal dailyAverage;

    [ObservableProperty]
    private ObservableCollection<CategorySpending> categoryBreakdown = new();

    [ObservableProperty]
    private ObservableCollection<MonthlyTrend> monthlyTrends = new();

    [ObservableProperty]
    private string topSpendingCategory = string.Empty;

    [ObservableProperty]
    private decimal averageDailySpending;
}

public partial class CategorySpending : ObservableObject
{
    [ObservableProperty]
    private string categoryName = string.Empty;

    [ObservableProperty]
    private decimal amount;

    [ObservableProperty]
    private decimal percentage;

    [ObservableProperty]
    private string color = string.Empty;

    [ObservableProperty]
    private int transactionCount;
}

public partial class MonthlyTrend : ObservableObject
{
    [ObservableProperty]
    private DateTime month;

    [ObservableProperty]
    private decimal income;

    [ObservableProperty]
    private decimal expenses;

    [ObservableProperty]
    private decimal netAmount;

    [ObservableProperty]
    private string monthName = string.Empty;
}

public partial class FinancialGoalProgress : ObservableObject
{
    [ObservableProperty]
    private string goalName = string.Empty;

    [ObservableProperty]
    private decimal targetAmount;

    [ObservableProperty]
    private decimal currentAmount;

    [ObservableProperty]
    private decimal progressPercentage;

    [ObservableProperty]
    private DateTime targetDate;

    [ObservableProperty]
    private int daysRemaining;

    [ObservableProperty]
    private decimal monthlyRequiredSavings;

    [ObservableProperty]
    private bool isOnTrack;
}

public enum TimePeriod
{
    Week,
    Month,
    Quarter,
    Year,
    Custom
}

public partial class TransactionFilter : ObservableObject
{
    [ObservableProperty]
    private DateTime startDate = DateTime.Now.AddMonths(-1);

    [ObservableProperty]
    private DateTime endDate = DateTime.Now;

    [ObservableProperty]
    private string selectedCategory = "All";

    [ObservableProperty]
    private TimePeriod timePeriod = TimePeriod.Month;

    [ObservableProperty]
    private decimal minAmount;

    [ObservableProperty]
    private decimal maxAmount = 10000;
}
