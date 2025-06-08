using Microcharts;
using Microsoft.Maui.Controls;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PersonalFinanceTracker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;

        private decimal _monthlyIncome;
        private decimal _monthlyExpense;
        private decimal _balance;

        public decimal MonthlyIncome
        {
            get => _monthlyIncome;
            set => SetProperty(ref _monthlyIncome, value);
        }

        public decimal MonthlyExpense
        {
            get => _monthlyExpense;
            set => SetProperty(ref _monthlyExpense, value);
        }

        public decimal Balance
        {
            get => _balance;
            set => SetProperty(ref _balance, value);
        }

        public ObservableCollection<Transaction> Transactions { get; set; }
        
        public ObservableCollection<Goal> Goals { get; set; }


        public Chart ExpenseChart { get; private set; }

        public ICommand AddTransactionCommand { get; }
        public ICommand OpenHistoryCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand OpenGoalsCommand { get; }
        public ICommand OpenBudgetsCommand { get; }

        public MainViewModel()
        {
            _databaseService = new DatabaseService(Path.Combine(FileSystem.AppDataDirectory, "financedb.db"));
            Transactions = new ObservableCollection<Transaction>();
            Goals = new ObservableCollection<Goal>();

            AddTransactionCommand = new Command(OnAddTransaction);
            OpenHistoryCommand = new Command(OnOpenHistory);
            OpenSettingsCommand = new Command(OnOpenSettings);
            OpenGoalsCommand = new Command(OnOpenGoals);
            OpenBudgetsCommand = new Command(OnOpenBudgets);

            LoadData();
        }

        public void LoadData()
        {
            Transactions.Clear();
            var all = _databaseService.GetAll();

            foreach (var t in all)
                Transactions.Add(t);

            var now = DateTime.Now;
            var currentMonthTransactions = all.Where(t => t.Date.Month == now.Month && t.Date.Year == now.Year);

            MonthlyIncome = currentMonthTransactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            MonthlyExpense = currentMonthTransactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            Balance = all.Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount);
            UpdateChart();
            // Загрузка целей
            LoadGoals();
        }

        private void LoadGoals()
        {
            Goals.Clear();
            var goalsFromDb = _databaseService.GetGoals();
            foreach (var goal in goalsFromDb)
            {
                Goals.Add(goal);
            }
            OnPropertyChanged(nameof(Goals));
        }

        private void UpdateChart()
        {
            var entries = Transactions
                .Where(t => t.Type == TransactionType.Expense)
                .GroupBy(t => t.Category)
                .Select((g, index) => new ChartEntry((float)g.Sum(t => t.Amount))
                {
                    Label = g.Key,
                    ValueLabel = g.Sum(t => t.Amount).ToString("F0") + "₽",
                    Color = GetColorForCategory(g.Key)
                }).ToList();

            ExpenseChart = new DonutChart
            {
                Entries = entries,
                LabelMode = LabelMode.RightOnly,
                LabelTextSize = 30,
                //BackgroundColor = SKColor.Parse("#E6F2EE"),
                AnimationProgress = 1
            };

            OnPropertyChanged(nameof(ExpenseChart));
        }

        private SKColor GetColorForCategory(string category)
        {
            return category switch
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
        }

        private async void OnAddTransaction()
        {
            await Shell.Current.GoToAsync("AddTransactionPage");
            UpdateChart();
        }

        private async void OnOpenHistory()
        {
            await Shell.Current.GoToAsync("TransactionListPage");
        }

        private async void OnOpenSettings()
        {
            await Shell.Current.GoToAsync("SettingsPage");
        }

        private async void OnOpenGoals()
        {
            await Shell.Current.DisplayAlert("Цели", "Переход к целям пока не реализован", "OK");
        }

        private async void OnOpenBudgets()
        {
            await Shell.Current.DisplayAlert("Бюджеты", "Переход к бюджетам пока не реализован", "OK");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string name = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(name);
            return true;
        }
    }
}
