using PersonalFinanceTracker.ViewModels;

namespace PersonalFinanceTracker.Views;

public partial class BudgetsPage : ContentPage
{
	public BudgetsPage(BudgetViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is BudgetViewModel vm)
		{
			await vm.LoadBudgetsCommand.ExecuteAsync(null);
		}
	}
}
