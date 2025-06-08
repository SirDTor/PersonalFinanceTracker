using PersonalFinanceTracker.Views;

namespace PersonalFinanceTracker
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(TransactionListPage), typeof(TransactionListPage));
            Routing.RegisterRoute(nameof(AddTransactionPage), typeof(AddTransactionPage));
            Routing.RegisterRoute(nameof(EditTransactionPage), typeof(EditTransactionPage));
            Routing.RegisterRoute(nameof(ExportPage), typeof(ExportPage));
            Routing.RegisterRoute(nameof(PinSettingsPage), typeof(PinSettingsPage));
            Routing.RegisterRoute(nameof(PinUnlockPage), typeof(PinUnlockPage));
        }
    }
}
