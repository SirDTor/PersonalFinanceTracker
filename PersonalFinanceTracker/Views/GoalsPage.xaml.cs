using PersonalFinanceTracker.Services;
using PersonalFinanceTracker.ViewModels;

namespace PersonalFinanceTracker.Views;

public partial class GoalsPage : ContentPage
{
    private readonly GoalViewModel viewModel;

    public GoalsPage(DatabaseService databaseService)
    {
        InitializeComponent();        
        viewModel = new GoalViewModel(databaseService);
        BindingContext = viewModel;
    }

    private async void OnAddGoalClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddGoalPage(viewModel)); // Теперь доступен
    }
}

