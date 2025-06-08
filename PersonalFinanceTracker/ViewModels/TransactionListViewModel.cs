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
        private readonly DatabaseService _databaseService;

        public ObservableCollection<Transaction> Transactions { get; set; }

        public ICommand DeleteCommand { get; }
        
        public ICommand EditCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public TransactionListViewModel(DatabaseService db)
        {
            _databaseService = db;
            DeleteCommand = new Command<Transaction>(DeleteTransaction);
            EditCommand = new Command<Transaction>(EditTransaction);
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            var all = _databaseService.GetAll();
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

            _databaseService.Delete(transaction);
            Transactions.Remove(transaction);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
