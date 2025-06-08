using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;

namespace PersonalFinanceTracker.ViewModels
{
    public class EditTransactionViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _db;
        public Transaction Transaction { get; set; }

        public string AmountText
        {
            get => Transaction.Amount.ToString("F2");
            set
            {
                if (decimal.TryParse(value, out var amount))
                    Transaction.Amount = amount;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public EditTransactionViewModel(Transaction transaction, DatabaseService db)
        {
            Transaction = transaction;
            _db = db;
            SaveCommand = new Command(Save);
        }

        private async void Save()
        {
            _db.Update(Transaction); // Предполагается, что метод Update реализован
            await Shell.Current.GoToAsync("..");
        }

        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
