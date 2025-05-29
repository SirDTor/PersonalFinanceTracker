
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;

namespace PersonalFinanceTracker.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<Transaction> Transactions { get; set; } = new();
        private DatabaseService _database;

        public MainViewModel(DatabaseService database)
        {
            _database = database;
        }

        public Task LoadTransactionsAsync()
        {
            var list = _database.GetAll();
            Transactions.Clear();
            foreach (var item in list)
                Transactions.Add(item);
            return Task.CompletedTask;
        }
    }
}
