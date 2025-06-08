using PersonalFinanceTracker.Services;
using PersonalFinanceTracker.ViewModels;

namespace PersonalFinanceTracker.Views
{
    public partial class AddTransactionPage : ContentPage
    {
        public AddTransactionPage(DatabaseService db)
        {
            InitializeComponent();
            BindingContext = new AddTransactionViewModel(db);
        }
    }
}
