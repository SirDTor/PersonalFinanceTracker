using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using PersonalFinanceTracker.ViewModels;

namespace PersonalFinanceTracker.Views
{
    [QueryProperty(nameof(Transaction), "Transaction")]
    public partial class EditTransactionPage : ContentPage
    {
        public Transaction Transaction
        {
            get => BindingContext is EditTransactionViewModel vm ? vm.Transaction : null;
            set => BindingContext = new EditTransactionViewModel(value, new DatabaseService(Path.Combine(FileSystem.AppDataDirectory, "financedb.db")));
        }

        public EditTransactionPage()
        {
            InitializeComponent();
        }
    }
}
