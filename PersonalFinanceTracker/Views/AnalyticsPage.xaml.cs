using PersonalFinanceTracker.ViewModels;

namespace PersonalFinanceTracker.Views;

public partial class AnalyticsPage : ContentPage
{
    public AnalyticsPage(AnalyticsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
