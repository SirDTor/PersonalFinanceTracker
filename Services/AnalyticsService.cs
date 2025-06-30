using PersonalFinanceTracker.Models;
using System.Collections.ObjectModel;

namespace PersonalFinanceTracker.Services;

public interface IAnalyticsService
{
    Task<SpendingAnalytics> GetSpendingAnalyticsAsync(DateTime startDate, DateTime endDate);
    Task<ObservableCollection<CategorySpending>> GetCategoryBreakdownAsync(DateTime startDate, DateTime endDate);
    Task<ObservableCollection<MonthlyTrend>> GetMonthlyTrendsAsync(int months = 12);
    Task<ObservableCollection<FinancialGoalProgress>> GetGoalProgressAsync();
    Task<ObservableCollection<Transaction>> GetFilteredTransactionsAsync(TransactionFilter filter);
    Task<Dictionary<string, decimal>> GetMonthlyComparisonAsync(DateTime currentMonth, DateTime previousMonth);
    Task<List<string>> GetSpendingInsightsAsync(DateTime startDate, DateTime endDate);
}

public class AnalyticsService : IAnalyticsService
{
    private readonly IDatabaseService _databaseService;
    private readonly string[] _chartColors = {
        "#FF6B6B", "#4ECDC4", "#45B7D1", "#96CEB4", "#FECA57",
        "#FF9F43", "#EE5A24", "#0FB9B1", "#2D98DA", "#20BF6B",
        "#F79F1F", "#A3CB38", "#6C5CE7", "#A55EEA", "#26DE81"
    };

    public AnalyticsService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<SpendingAnalytics> GetSpendingAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        var transactions = await _databaseService.GetTransactionsAsync();
        var filteredTransactions = transactions.Where(t => t.Date >= startDate && t.Date <= endDate).ToList();

        var totalIncome = filteredTransactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var totalExpenses = filteredTransactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
        var netAmount = totalIncome - totalExpenses;
        var savingsRate = totalIncome > 0 ? (netAmount / totalIncome) * 100 : 0;

        var categoryBreakdown = await GetCategoryBreakdownAsync(startDate, endDate);
        var monthlyTrends = await GetMonthlyTrendsAsync(3); // Last 3 months

        var topCategory = categoryBreakdown.OrderByDescending(c => c.Amount).FirstOrDefault()?.CategoryName ?? "None";
        var days = (endDate - startDate).Days;
        var averageDailySpending = days > 0 ? totalExpenses / days : 0;

        return new SpendingAnalytics
        {
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            NetAmount = netAmount,
            SavingsRate = savingsRate,
            CategoryBreakdown = categoryBreakdown,
            MonthlyTrends = monthlyTrends,
            TopSpendingCategory = topCategory,
            AverageDailySpending = averageDailySpending
        };
    }

    public async Task<ObservableCollection<CategorySpending>> GetCategoryBreakdownAsync(DateTime startDate, DateTime endDate)
    {
        var transactions = await _databaseService.GetTransactionsAsync();
        var expenses = transactions.Where(t => t.Type == TransactionType.Expense && 
                                             t.Date >= startDate && 
                                             t.Date <= endDate).ToList();

        var totalExpenses = expenses.Sum(t => t.Amount);
        
        var categoryGroups = expenses.GroupBy(t => t.Category)
                                   .Select((group, index) => new CategorySpending
                                   {
                                       CategoryName = group.Key,
                                       Amount = group.Sum(t => t.Amount),
                                       TransactionCount = group.Count(),
                                       Color = _chartColors[index % _chartColors.Length],
                                       Percentage = totalExpenses > 0 ? (group.Sum(t => t.Amount) / totalExpenses) * 100 : 0
                                   })
                                   .OrderByDescending(c => c.Amount)
                                   .ToList();

        return new ObservableCollection<CategorySpending>(categoryGroups);
    }

    public async Task<ObservableCollection<MonthlyTrend>> GetMonthlyTrendsAsync(int months = 12)
    {
        var transactions = await _databaseService.GetTransactionsAsync();
        var startDate = DateTime.Now.AddMonths(-months);

        var monthlyData = transactions.Where(t => t.Date >= startDate)
                                    .GroupBy(t => new { t.Date.Year, t.Date.Month })
                                    .Select(group =>
                                    {
                                        var monthDate = new DateTime(group.Key.Year, group.Key.Month, 1);
                                        var income = group.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
                                        var expenses = group.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
                                        
                                        return new MonthlyTrend
                                        {
                                            Month = monthDate,
                                            MonthName = monthDate.ToString("MMM yyyy"),
                                            Income = income,
                                            Expenses = expenses,
                                            NetAmount = income - expenses
                                        };
                                    })
                                    .OrderBy(m => m.Month)
                                    .ToList();

        return new ObservableCollection<MonthlyTrend>(monthlyData);
    }

    public async Task<ObservableCollection<FinancialGoalProgress>> GetGoalProgressAsync()
    {
        var goals = await _databaseService.GetGoalsAsync();
        var progressList = new List<FinancialGoalProgress>();

        foreach (var goal in goals)
        {
            var daysRemaining = (goal.TargetDate - DateTime.Now).Days;
            var monthsRemaining = Math.Max(1, daysRemaining / 30.0);
            var remainingAmount = Math.Max(0, goal.TargetAmount - goal.CurrentAmount);
            var monthlyRequired = remainingAmount / (decimal)monthsRemaining;
            
            var progress = new FinancialGoalProgress
            {
                GoalName = goal.Name,
                TargetAmount = goal.TargetAmount,
                CurrentAmount = goal.CurrentAmount,
                ProgressPercentage = goal.TargetAmount > 0 ? (goal.CurrentAmount / goal.TargetAmount) * 100 : 0,
                TargetDate = goal.TargetDate,
                DaysRemaining = Math.Max(0, daysRemaining),
                MonthlyRequiredSavings = monthlyRequired,
                IsOnTrack = goal.CurrentAmount >= (goal.TargetAmount * (1 - (daysRemaining / (goal.TargetDate - DateTime.Now.AddYears(-1)).TotalDays)))
            };

            progressList.Add(progress);
        }

        return new ObservableCollection<FinancialGoalProgress>(progressList.OrderByDescending(g => g.ProgressPercentage));
    }

    public async Task<ObservableCollection<Transaction>> GetFilteredTransactionsAsync(TransactionFilter filter)
    {
        var transactions = await _databaseService.GetTransactionsAsync();

        var filtered = transactions.Where(t =>
            t.Date >= filter.StartDate &&
            t.Date <= filter.EndDate &&
            (filter.SelectedCategory == "All" || t.Category == filter.SelectedCategory) &&
            t.Amount >= filter.MinAmount &&
            t.Amount <= filter.MaxAmount)
            .OrderByDescending(t => t.Date)
            .ToList();

        return new ObservableCollection<Transaction>(filtered);
    }

    public async Task<Dictionary<string, decimal>> GetMonthlyComparisonAsync(DateTime currentMonth, DateTime previousMonth)
    {
        var transactions = await _databaseService.GetTransactionsAsync();

        var currentMonthData = transactions.Where(t => 
            t.Date.Year == currentMonth.Year && 
            t.Date.Month == currentMonth.Month).ToList();

        var previousMonthData = transactions.Where(t => 
            t.Date.Year == previousMonth.Year && 
            t.Date.Month == previousMonth.Month).ToList();

        return new Dictionary<string, decimal>
        {
            ["CurrentIncome"] = currentMonthData.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
            ["CurrentExpenses"] = currentMonthData.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
            ["PreviousIncome"] = previousMonthData.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
            ["PreviousExpenses"] = previousMonthData.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
        };
    }

    public async Task<List<string>> GetSpendingInsightsAsync(DateTime startDate, DateTime endDate)
    {
        var analytics = await GetSpendingAnalyticsAsync(startDate, endDate);
        var insights = new List<string>();

        // Savings rate insights
        if (analytics.SavingsRate > 20)
            insights.Add($"Great job! You're saving {analytics.SavingsRate:F1}% of your income.");
        else if (analytics.SavingsRate > 10)
            insights.Add($"You're saving {analytics.SavingsRate:F1}% - consider increasing to 20% for better financial health.");
        else if (analytics.SavingsRate > 0)
            insights.Add($"Your savings rate of {analytics.SavingsRate:F1}% could be improved. Try to save at least 10%.");
        else
            insights.Add("You're spending more than you earn. Consider reviewing your expenses.");

        // Category insights
        if (analytics.CategoryBreakdown.Any())
        {
            var topCategory = analytics.CategoryBreakdown.OrderByDescending(c => c.Amount).First();
            if (topCategory.Percentage > 30)
                insights.Add($"{topCategory.CategoryName} accounts for {topCategory.Percentage:F1}% of spending - consider if this is appropriate.");
        }

        // Spending pattern insights
        if (analytics.AverageDailySpending > 100)
            insights.Add($"Your average daily spending is ${analytics.AverageDailySpending:F2} - track daily expenses more closely.");

        // Monthly trend insights
        if (analytics.MonthlyTrends.Count >= 2)
        {
            var currentMonth = analytics.MonthlyTrends.Last();
            var previousMonth = analytics.MonthlyTrends[analytics.MonthlyTrends.Count - 2];
            
            var expenseChange = ((currentMonth.Expenses - previousMonth.Expenses) / previousMonth.Expenses) * 100;
            if (expenseChange > 10)
                insights.Add($"Your expenses increased by {expenseChange:F1}% from last month.");
            else if (expenseChange < -10)
                insights.Add($"Great! You reduced expenses by {Math.Abs(expenseChange):F1}% from last month.");
        }

        return insights;
    }
}
