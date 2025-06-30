using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microcharts;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Globalization;

namespace PersonalFinanceTracker.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IDatabaseService _databaseService;

        [ObservableProperty] decimal monthlyIncome;
        [ObservableProperty] decimal monthlyExpense;
        [ObservableProperty] decimal balance;
        [ObservableProperty] Chart? expenseChart;
        [ObservableProperty] ObservableCollection<Transaction> recentTransactions = new();
        [ObservableProperty] ObservableCollection<Goal> goals = new();
        [ObservableProperty] ObservableCollection<Budget> activeBudgets = new();
        [ObservableProperty] bool hasOverdueBudgets;
        [ObservableProperty] int overdueRecurringCount;

        public MainViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Главная";
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await Task.WhenAll(
                    LoadTransactionsAsync(),
                    LoadGoalsAsync(),
                    LoadBudgetsAsync(),
                    LoadRecurringTransactionsAsync()
                );
                UpdateChart();
            }
            catch (Exception ex)
            {
                // TODO: можно подключить IMessageService.ShowError(ex.Message);
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки данных: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadTransactionsAsync()
        {
            await Task.Run(() =>
            {
                var all = _databaseService.GetAllTransactions();
                var recent = all.Take(5).ToList();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    RecentTransactions.Clear();
                    foreach (var transaction in recent)
                        RecentTransactions.Add(transaction);
                });

                var now = DateTime.Now;
                var currentMonthTransactions = all.Where(t => t.Date.Month == now.Month && t.Date.Year == now.Year);

                MonthlyIncome = currentMonthTransactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Amount);

                MonthlyExpense = currentMonthTransactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Amount);

                Balance = _databaseService.GetBalance();
            });
        }

        private async Task LoadGoalsAsync()
        {
            await Task.Run(() =>
            {
                var goalsFromDb = _databaseService.GetGoals().Take(3).ToList();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Goals.Clear();
                    foreach (var goal in goalsFromDb)
                        Goals.Add(goal);
                });
            });
        }

        private async Task LoadBudgetsAsync()
        {
            await Task.Run(() =>
            {
                var budgets = _databaseService.GetActiveBudgets();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ActiveBudgets.Clear();
                    foreach (var budget in budgets)
                        ActiveBudgets.Add(budget);

                    HasOverdueBudgets = budgets.Any(b => b.IsOverBudget);
                });
            });
        }

        private async Task LoadRecurringTransactionsAsync()
        {
            await Task.Run(() =>
            {
                var overdueCount = _databaseService.GetDueRecurringTransactions().Count;
                OverdueRecurringCount = overdueCount;
            });
        }

        private void UpdateChart()
        {
            var entries = RecentTransactions
                .Where(t => t.Type == TransactionType.Expense)
                .GroupBy(t => t.Category)
                .Select(g => new ChartEntry((float)g.Sum(t => t.Amount))
                {
                    Label = g.Key,
                    ValueLabel = g.Sum(t => t.Amount).ToString("C0", CultureInfo.CurrentCulture),
                    Color = GetColorForCategory(g.Key)
                }).ToList();

            ExpenseChart = entries.Any()
                ? new DonutChart
                {
                    Entries = entries,
                    LabelMode = LabelMode.RightOnly,
                    LabelTextSize = 30,
                    AnimationProgress = 1
                }
                : null;
        }

        private SKColor GetColorForCategory(string category) =>
            category switch
            {
                "Еда" => SKColor.Parse("#FF6F61"),
                "Транспорт" => SKColor.Parse("#6B5B95"),
                "Развлечения" => SKColor.Parse("#88B04B"),
                "Здоровье" => SKColor.Parse("#FFA07A"),
                "Поступление средств" => SKColor.Parse("#6BE53E"),
                "Перевод средств" => SKColor.Parse("#E5973E"),
                "Другое" => SKColor.Parse("#00ACC1"),
                _ => SKColor.Parse("#607D8B")
            };

        [RelayCommand]
        private async Task AddTransactionAsync() =>
            await Shell.Current.GoToAsync("AddTransactionPage");

        [RelayCommand]
        private async Task OpenHistoryAsync() =>
            await Shell.Current.GoToAsync("TransactionListPage");

        [RelayCommand]
        private async Task OpenSettingsAsync() =>
            await Shell.Current.GoToAsync("SettingsPage");

        [RelayCommand]
        private async Task OpenGoalsAsync() =>
            await Shell.Current.GoToAsync("GoalsPage");

        [RelayCommand]
        private async Task OpenBudgetsAsync() =>
            await Shell.Current.GoToAsync("BudgetsPage");

        [RelayCommand]
        private async Task RefreshAsync() =>
            await LoadDataAsync();
    }
}
