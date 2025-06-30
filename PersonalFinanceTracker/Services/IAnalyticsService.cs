using PersonalFinanceTracker.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalFinanceTracker.Services;

/// <summary>
/// Сервис для аналитики финансовых данных, включая расходы по категориям,
/// тренды, цели и генерацию инсайтов.
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// Получает агрегированную аналитику по доходам, расходам и сбережениям
    /// за указанный период.
    /// </summary>
    Task<SpendingAnalytics> GetSpendingAnalyticsAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Возвращает детализацию расходов по категориям за указанный период,
    /// включая проценты и цвета для диаграмм.
    /// </summary>
    Task<ObservableCollection<CategorySpending>> GetCategoryBreakdownAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Возвращает тренды доходов, расходов и чистых остатков за каждый месяц
    /// за последние N месяцев.
    /// </summary>
    Task<ObservableCollection<MonthlyTrend>> GetMonthlyTrendsAsync(int months = 12);

    /// <summary>
    /// Возвращает прогресс выполнения финансовых целей пользователя.
    /// </summary>
    Task<ObservableCollection<FinancialGoalProgress>> GetGoalProgressAsync();

    /// <summary>
    /// Возвращает список транзакций, отфильтрованных по заданным параметрам.
    /// </summary>
    Task<ObservableCollection<Transaction>> GetFilteredTransactionsAsync(TransactionFilter filter);

    /// <summary>
    /// Сравнивает доходы и расходы текущего месяца с предыдущим.
    /// </summary>
    /// <returns>
    /// Словарь с ключами:
    /// "CurrentIncome", "CurrentExpenses", "PreviousIncome", "PreviousExpenses".
    /// </returns>
    Task<Dictionary<string, decimal>> GetMonthlyComparisonAsync(DateTime currentMonth, DateTime previousMonth);

    /// <summary>
    /// Генерирует список текстовых инсайтов на основе анализа расходов
    /// за указанный период.
    /// </summary>
    Task<List<string>> GetSpendingInsightsAsync(DateTime startDate, DateTime endDate);
}
