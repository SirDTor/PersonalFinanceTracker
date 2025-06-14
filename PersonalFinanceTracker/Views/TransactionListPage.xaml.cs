using PersonalFinanceTracker.Services;
using PersonalFinanceTracker.ViewModels;

namespace PersonalFinanceTracker.Views
{
    public partial class TransactionListPage : ContentPage
    {
        private readonly TransactionListViewModel _viewModel;

        public TransactionListPage(DatabaseService db)
        {
            InitializeComponent();
            _viewModel = new TransactionListViewModel(db);
            BindingContext = _viewModel;
        }
    }
}
