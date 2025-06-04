using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using Microcharts;
using SkiaSharp;

namespace PersonalFinanceTracker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _dbService;

        public Chart ExpenseChart { get; private set; }

        public ObservableCollection<Transaction> Transactions { get; private set; }

        private decimal _balance;
        public decimal Balance
        {
            get => _balance;
            set
            {
                if (_balance != value)
                {
                    _balance = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
            LoadTransactions();
        }

        public void LoadTransactions()
        {
            var list = _dbService.GetAll();
            Transactions = new ObservableCollection<Transaction>(list);
            UpdateBalance();
            UpdateChart();
            OnPropertyChanged(nameof(Transactions));
        }

        public void AddTransaction(Transaction transaction)
        {
            _dbService.Add(transaction);
            Transactions.Insert(0, transaction);
            UpdateBalance();
            UpdateChart();
        }

        public void DeleteTransaction(Transaction transaction)
        {
            _dbService.Delete(transaction);
            Transactions.Remove(transaction);
            UpdateBalance();
            UpdateChart();
        }

        private void UpdateBalance()
        {
            decimal income = Transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            decimal expense = Transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
            Balance = income - expense;
        }

        private void UpdateChart()
        {
            var entries = Transactions
                //.Where(t => t.Type == TransactionType.Expense)
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
                BackgroundColor = SKColor.Parse("#E6F2EE"),
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
