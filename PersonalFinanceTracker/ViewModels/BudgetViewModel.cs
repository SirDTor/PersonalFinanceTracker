using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using System.Collections.ObjectModel;

namespace PersonalFinanceTracker.ViewModels
{
    public partial class BudgetViewModel : BaseViewModel
    {
        private readonly IDatabaseService _databaseService;

        [ObservableProperty]
        ObservableCollection<Budget> budgets = new();

        [ObservableProperty]
        Budget selectedBudget = new();

        [ObservableProperty]
        bool isAddingBudget;

        [ObservableProperty]
        string budgetName = string.Empty;

        [ObservableProperty]
        string budgetCategory = string.Empty;

        [ObservableProperty]
        decimal budgetAmount;

        [ObservableProperty]
        BudgetPeriod selectedPeriod = BudgetPeriod.Monthly;

        [ObservableProperty]
        DateTime startDate = DateTime.Now;

        [ObservableProperty]
        DateTime endDate = DateTime.Now.AddMonths(1);

        public ObservableCollection<BudgetPeriod> BudgetPeriods { get; } = new()
        {
            BudgetPeriod.Weekly,
            BudgetPeriod.Monthly,
            BudgetPeriod.Quarterly,
            BudgetPeriod.Yearly
        };

        public ObservableCollection<string> Categories { get; } = new()
        {
            "Еда", "Транспорт", "Развлечения", "Здоровье", "Жилье", "Одежда", "Образование", "Другое"
        };

        public BudgetViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Бюджеты";
        }

        [RelayCommand]
        private async Task LoadBudgetsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                
                await Task.Run(() =>
                {
                    var budgetList = _databaseService.GetBudgets();
                    
                    // Update spent amounts
                    foreach (var budget in budgetList)
                    {
                        var transactions = _databaseService.GetTransactionsByDateRange(budget.StartDate, budget.EndDate)
                            .Where(t => t.Category == budget.Category && t.Type == TransactionType.Expense);
                        budget.SpentAmount = transactions.Sum(t => t.Amount);
                        _databaseService.UpdateBudget(budget);
                    }

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Budgets.Clear();
                        foreach (var budget in budgetList.OrderByDescending(b => b.StartDate))
                            Budgets.Add(budget);
                    });
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void StartAddBudget()
        {
            ResetForm();
            IsAddingBudget = true;
        }

        [RelayCommand]
        private void CancelAddBudget()
        {
            IsAddingBudget = false;
            ResetForm();
        }

        [RelayCommand]
        private async Task SaveBudgetAsync()
        {
            if (string.IsNullOrWhiteSpace(BudgetName) || string.IsNullOrWhiteSpace(BudgetCategory) || BudgetAmount <= 0)
            {
                await Shell.Current.DisplayAlert("Ошибка", "Заполните все поля корректно", "ОК");
                return;
            }

            var budget = new Budget
            {
                Name = BudgetName,
                Category = BudgetCategory,
                BudgetAmount = BudgetAmount,
                Period = SelectedPeriod,
                StartDate = StartDate,
                EndDate = CalculateEndDate(StartDate, SelectedPeriod),
                SpentAmount = 0,
                IsActive = true
            };

            _databaseService.AddBudget(budget);
            await LoadBudgetsAsync();
            
            IsAddingBudget = false;
            ResetForm();
            
            await Shell.Current.DisplayAlert("Успех", "Бюджет создан", "ОК");
        }

        [RelayCommand]
        private async Task DeleteBudgetAsync(Budget budget)
        {
            if (budget == null) return;

            var result = await Shell.Current.DisplayAlert("Подтверждение", 
                $"Удалить бюджет '{budget.Name}'?", "Да", "Нет");

            if (result)
            {
                _databaseService.DeleteBudget(budget);
                await LoadBudgetsAsync();
            }
        }

        [RelayCommand]
        private async Task ToggleBudgetStatusAsync(Budget budget)
        {
            if (budget == null) return;

            budget.IsActive = !budget.IsActive;
            _databaseService.UpdateBudget(budget);
            await LoadBudgetsAsync();
        }

        private void ResetForm()
        {
            BudgetName = string.Empty;
            BudgetCategory = string.Empty;
            BudgetAmount = 0;
            SelectedPeriod = BudgetPeriod.Monthly;
            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddMonths(1);
        }

        private static DateTime CalculateEndDate(DateTime startDate, BudgetPeriod period)
        {
            return period switch
            {
                BudgetPeriod.Weekly => startDate.AddDays(7),
                BudgetPeriod.Monthly => startDate.AddMonths(1),
                BudgetPeriod.Quarterly => startDate.AddMonths(3),
                BudgetPeriod.Yearly => startDate.AddYears(1),
                _ => startDate.AddMonths(1)
            };
        }

        partial void OnSelectedPeriodChanged(BudgetPeriod value)
        {
            EndDate = CalculateEndDate(StartDate, value);
        }

        partial void OnStartDateChanged(DateTime value)
        {
            EndDate = CalculateEndDate(value, SelectedPeriod);
        }
    }
}
