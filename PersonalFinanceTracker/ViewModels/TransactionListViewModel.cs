using CommunityToolkit.Mvvm.Input;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PersonalFinanceTracker.ViewModels
{
    public class TransactionListViewModel : INotifyPropertyChanged
    {
        private readonly IDatabaseService _databaseService;

        public ObservableCollection<Transaction> Transactions { get; set; }
        public ObservableCollection<string> Categories { get; set; } = new();

        private string _selectedCategory = "Все";
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime _endDate = DateTime.Now;
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ApplyFilterCommand { get; }

        public ICommand DeleteCommand { get; }
        
        public ICommand EditCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public TransactionListViewModel(IDatabaseService db)
        {
            _databaseService = db;
            DeleteCommand = new RelayCommand<Transaction>(DeleteTransaction);
            EditCommand = new RelayCommand<Transaction>(EditTransaction);
            ApplyFilterCommand = new RelayCommand(ApplyFilter);
            LoadTransactions();
            LoadCategories();
            ApplyFilter();
        }

        private void LoadTransactions()
        {
            var all = _databaseService.GetAllTransactions();
            Transactions = new ObservableCollection<Transaction>(all);
            OnPropertyChanged(nameof(Transactions));
        }

        private async void EditTransaction(Transaction transaction)
        {
            if (transaction == null) return;

            await Shell.Current.GoToAsync("EditTransactionPage", new Dictionary<string, object>
            {
                { "Transaction", transaction }
            });
        }

        private async void DeleteTransaction(Transaction transaction)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Удаление", "Удалить транзакцию?", "Да", "Нет");
            if (!confirm) return;

            _databaseService.DeleteTransaction(transaction);
            Transactions.Remove(transaction);
        }

        private void LoadCategories()
        {
            var allTransactions = _databaseService.GetAllTransactions();
            var categories = allTransactions.Select(t => t.Category).Distinct().OrderBy(c => c).ToList();

            Categories.Clear();
            Categories.Add("Все");
            foreach (var cat in categories.Where(c => !string.IsNullOrWhiteSpace(c)))
                Categories.Add(cat);
        }

        private void ApplyFilter()
        {
            var filtered = _databaseService.GetAllTransactions()
                .Where(t => t.Date >= StartDate && t.Date <= EndDate);

            if (SelectedCategory != "Все")
                filtered = filtered.Where(t => t.Category == SelectedCategory);

            Transactions.Clear();
            foreach (var t in filtered.OrderByDescending(t => t.Date))
                Transactions.Add(t);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
