using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;

namespace PersonalFinanceTracker.ViewModels
{
    public class TransactionListViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<Transaction> Transactions { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public TransactionListViewModel(DatabaseService db)
        {
            _databaseService = db;
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            var all = _databaseService.GetAll();
            Transactions = new ObservableCollection<Transaction>(all);
            OnPropertyChanged(nameof(Transactions));
        }

        public void DeleteTransaction(Transaction transaction)
        {
            _databaseService.Delete(transaction);
            Transactions.Remove(transaction);
            OnPropertyChanged(nameof(Transactions));
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
