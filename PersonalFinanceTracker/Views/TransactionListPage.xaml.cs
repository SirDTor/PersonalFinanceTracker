using PersonalFinanceTracker.Services;

namespace PersonalFinanceTracker.Views;

public partial class TransactionListPage : ContentPage
{
    private DatabaseService _dataService;

    public TransactionListPage()
    {
        InitializeComponent();
        _dataService = new DatabaseService(Path.Combine(FileSystem.AppDataDirectory, "financedb.db"));
        LoadTransactions();
    }

    private void LoadTransactions()
    {
        TransactionList.ItemsSource = _dataService.GetAll();
    }
}
