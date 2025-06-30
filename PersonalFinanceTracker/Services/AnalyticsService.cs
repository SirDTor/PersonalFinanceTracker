using PersonalFinanceTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceTracker.Services;

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
        var transactions = await _databaseService.GetTransactionsAsync().ConfigureAwait(false);
        var filteredTransactions = FilterTransactions(transactions, new TransactionFilter
        {
            StartDate = startDate,
            EndDate = endDate
        });

        var income = filteredTransactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var expenses = filteredTransactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
        var netAmount = income - expenses;
        var savingsRate = income != 0 ? (netAmount / income) * 100 : 0;

        var categoryBreakdown = await GetCategoryBreakdownAsync(startDate, endDate).ConfigureAwait(false);
        var monthlyTrends = await GetMonthlyTrendsAsync(3).ConfigureAwait(false);

        var topCategory = categoryBreakdown.FirstOrDefault()?.CategoryName ?? "None";
        var totalDays = (endDate - startDate).Days;
        var averageDailySpending = totalDays > 0 ? expenses / totalDays : 0;

        return new SpendingAnalytics
        {
            TotalIncome = income,
            TotalExpenses = expenses,
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
        var transactions = await _databaseService.GetTransactionsAsync().ConfigureAwait(false);
        var expenses = transactions.Where(t => t.Type == TransactionType.Expense &&
                                               t.Date >= startDate &&
                                               t.Date <= endDate)
                                   .ToList();

        var totalExpenses = expenses.Sum(t => t.Amount);

        var categoryGroups = expenses.GroupBy(t => t.Category)
                                     .Select((group, index) =>
                                     {
                                         var amount = group.Sum(t => t.Amount);
                                         return new CategorySpending
                                         {
                                             CategoryName = group.Key,
                                             Amount = amount,
                                             TransactionCount = group.Count(),
                                             Color = _chartColors[index % _chartColors.Length],
                                             Percentage = totalExpenses > 0 ? (amount / totalExpenses) * 100 : 0
                                         };
                                     })
                                     .OrderByDescending(c => c.Amount)
                                     .ToList();

        return new ObservableCollection<CategorySpending>(categoryGroups);
    }

    public async Task<ObservableCollection<MonthlyTrend>> GetMonthlyTrendsAsync(int months = 12)
    {
        var transactions = await _databaseService.GetTransactionsAsync().ConfigureAwait(false);
        var startDate = DateTime.Now.AddMonths(-months);

        var grouped = transactions.Where(t => t.Date >= startDate)
                                  .GroupBy(t => new { t.Date.Year, t.Date.Month })
                                  .Select(group =>
                                  {
                                      var monthDate = new DateTime(group.Key.Year, group.Key.Month, 1);
                                      var income = group.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
                                      var expenses = group.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
                                      var netAmount = income - expenses;
                                      var savingsRate = income != 0 ? (netAmount / income) * 100 : 0;

                                      return new MonthlyTrend
                                      {
                                          Month = monthDate.ToString("MMM yyyy", CultureInfo.InvariantCulture),
                                          Date = monthDate,
                                          Income = income,
                                          Expenses = expenses,
                                          NetAmount = netAmount,
                                          SavingsRate = savingsRate,
                                          IsPositive = netAmount >= 0
                                      };
                                  })
                                  .OrderBy(g => g.Date)
                                  .ToList();

        return new ObservableCollection<MonthlyTrend>(grouped);
    }

    public async Task<ObservableCollection<FinancialGoalProgress>> GetGoalProgressAsync()
    {
        var goals = await _databaseService.GetGoalsAsync().ConfigureAwait(false);
        var now = DateTime.Now;

        var progressList = goals.Select(goal =>
        {
            var daysRemaining = (goal.TargetDate - now).Days;
            var remainingAmount = Math.Max(0, goal.TargetAmount - goal.CurrentAmount);
            var monthsRemaining = Math.Max(1, daysRemaining / 30.0);
            var requiredMonthly = remainingAmount / (decimal)monthsRemaining;
            var progress = goal.TargetAmount != 0 ? (goal.CurrentAmount / goal.TargetAmount) * 100 : 0;

            return new FinancialGoalProgress
            {
                GoalId = goal.Id,
                GoalName = goal.Name,
                TargetAmount = goal.TargetAmount,
                CurrentAmount = goal.CurrentAmount,
                Progress = progress,
                TargetDate = goal.TargetDate,
                DaysRemaining = Math.Max(0, daysRemaining),
                RequiredMonthlyContribution = requiredMonthly,
                IsOnTrack = progress >= 50, // можно динамичнее
                Status = progress switch
                {
                    >= 75 => "On Track",
                    >= 50 => "Slightly Behind",
                    _ => "At Risk"
                },
                StatusColor = progress switch
                {
                    >= 75 => "Green",
                    >= 50 => "Orange",
                    _ => "Red"
                }
            };
        }).OrderByDescending(g => g.Progress).ToList();

        return new ObservableCollection<FinancialGoalProgress>(progressList);
    }

    public async Task<ObservableCollection<Transaction>> GetFilteredTransactionsAsync(TransactionFilter filter)
    {
        var transactions = await _databaseService.GetTransactionsAsync().ConfigureAwait(false);
        var filtered = FilterTransactions(transactions, filter)
                       .OrderByDescending(t => t.Date)
                       .ToList();

        return new ObservableCollection<Transaction>(filtered);
    }

    public async Task<Dictionary<string, decimal>> GetMonthlyComparisonAsync(DateTime currentMonth, DateTime previousMonth)
    {
        var transactions = await _databaseService.GetTransactionsAsync().ConfigureAwait(false);

        decimal GetSum(IEnumerable<Transaction> tList, TransactionType type) =>
            tList.Where(t => t.Type == type).Sum(t => t.Amount);

        var currentData = transactions.Where(t => t.Date.Year == currentMonth.Year && t.Date.Month == currentMonth.Month).ToList();
        var previousData = transactions.Where(t => t.Date.Year == previousMonth.Year && t.Date.Month == previousMonth.Month).ToList();

        return new Dictionary<string, decimal>
        {
            ["CurrentIncome"] = GetSum(currentData, TransactionType.Income),
            ["CurrentExpenses"] = GetSum(currentData, TransactionType.Expense),
            ["PreviousIncome"] = GetSum(previousData, TransactionType.Income),
            ["PreviousExpenses"] = GetSum(previousData, TransactionType.Expense),
        };
    }

    public async Task<List<string>> GetSpendingInsightsAsync(DateTime startDate, DateTime endDate)
    {
        var analytics = await GetSpendingAnalyticsAsync(startDate, endDate).ConfigureAwait(false);
        var insights = new List<string>();

        if (analytics.SavingsRate > 20)
            insights.Add($"Great job! You're saving {analytics.SavingsRate:F1}% of your income.");
        else if (analytics.SavingsRate > 10)
            insights.Add($"You're saving {analytics.SavingsRate:F1}%. Aim for 20% for more security.");
        else
            insights.Add("You're saving very little or negative. Review your expenses.");

        var topCategory = analytics.CategoryBreakdown.FirstOrDefault();
        if (topCategory != null && topCategory.Percentage > 30)
            insights.Add($"{topCategory.CategoryName} takes {topCategory.Percentage:F1}% of your spending.");

        if (analytics.AverageDailySpending > 100)
            insights.Add($"Your daily spending is ${analytics.AverageDailySpending:F2} - consider tracking closely.");

        var trends = analytics.MonthlyTrends;
        if (trends.Count >= 2)
        {
            var last = trends[^1];
            var prev = trends[^2];
            var change = prev.Expenses != 0 ? ((last.Expenses - prev.Expenses) / prev.Expenses) * 100 : 0;

            if (change > 10)
                insights.Add($"Expenses up by {change:F1}% from last month.");
            else if (change < -10)
                insights.Add($"Great! Expenses down by {Math.Abs(change):F1}%.");
        }

        return insights;
    }

    private static IEnumerable<Transaction> FilterTransactions(IEnumerable<Transaction> transactions, TransactionFilter filter)
    {
        return transactions.Where(t =>
            (!filter.StartDate.HasValue || t.Date >= filter.StartDate.Value) &&
            (!filter.EndDate.HasValue || t.Date <= filter.EndDate.Value) &&
            (string.IsNullOrEmpty(filter.Category) || t.Category == filter.Category) &&
            (!filter.MinAmount.HasValue || t.Amount >= filter.MinAmount.Value) &&
            (!filter.MaxAmount.HasValue || t.Amount <= filter.MaxAmount.Value) &&
            (!filter.Type.HasValue || t.Type == filter.Type.Value)
        );
    }
}
