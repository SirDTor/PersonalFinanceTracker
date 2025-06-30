using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Models
{
    public class SpendingAnalytics
    {
        public decimal TotalSpent { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetAmount { get; set; }
        public decimal SavingsRate { get; set; }
        public decimal AverageTransaction { get; set; }
        public decimal AverageDailySpending { get; set; }
        public decimal DailyAverage { get; set; }
        public int TransactionCount { get; set; }
        public decimal MonthlyAverage { get; set; }
        public decimal ComparedToLastMonth { get; set; }
        public string TopSpendingCategory { get; set; } = "";
        
        public ObservableCollection<CategorySpending> CategoryBreakdown { get; set; } = new();
        public ObservableCollection<MonthlyTrend> MonthlyTrends { get; set; } = new();
    }
    public class CategorySpending
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
        public int TransactionCount { get; set; }
        public string Color { get; set; } = "#007ACC";
        public bool IsOverBudget { get; set; }
        public decimal BudgetAmount { get; set; }
    }

    public class MonthlyTrend
    {
        public string Month { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
        public decimal NetAmount { get; set; }
        public decimal SavingsRate { get; set; }
        public bool IsPositive { get; set; }
    }

    public class FinancialGoalProgress
    {
        public int GoalId { get; set; }
        public string GoalName { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal Progress { get; set; }
        public DateTime TargetDate { get; set; }
        public int DaysRemaining { get; set; }
        public bool IsOnTrack { get; set; }
        public decimal RequiredMonthlyContribution { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
    }

    public class TransactionFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Используем строку вместо списка ID
        public string? Category { get; set; }

        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }

        public string? SearchTerm { get; set; }
        public TransactionType? Type { get; set; }

        public bool IncludeRecurring { get; set; } = true;
    }


    public class FinancialInsight
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
        public InsightType Type { get; set; }
        public InsightPriority Priority { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public decimal? ImpactAmount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public enum InsightType
    {
        Spending,
        Saving,
        Budget,
        Goal,
        Trend,
        Alert
    }

    public enum InsightPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class BudgetAnalysis
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; } = string.Empty;
        public decimal BudgetAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal UtilizationPercentage { get; set; }
        public bool IsOverBudget { get; set; }
        public int DaysRemaining { get; set; }
        public decimal DailyAllowance { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public List<Transaction> RecentTransactions { get; set; } = new();
    }

    public class SavingsAnalysis
    {
        public decimal TotalSavings { get; set; }
        public decimal MonthlySavings { get; set; }
        public decimal SavingsRate { get; set; }
        public decimal ProjectedYearlySavings { get; set; }
        public List<MonthlyTrend> SavingsTrends { get; set; } = new();
        public decimal ComparedToLastMonth { get; set; }
        public bool IsImproving { get; set; }
        public List<FinancialGoalProgress> SavingsGoals { get; set; } = new();
    }
}
