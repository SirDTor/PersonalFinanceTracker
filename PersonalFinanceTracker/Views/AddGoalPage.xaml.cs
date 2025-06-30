using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.ViewModels;

namespace PersonalFinanceTracker.Views;

public partial class AddGoalPage : ContentPage
{
    private readonly GoalViewModel _viewModel;

    public AddGoalPage(GoalViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        TargetDatePicker.Date = DateTime.Now.AddMonths(1);
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string title = TitleEntry.Text?.Trim();
        string amountText = TargetAmountEntry.Text;

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(amountText) || !decimal.TryParse(amountText, out decimal amount))
        {
            await DisplayAlert("Ошибка", "Пожалуйста, заполните все поля корректно.", "ОК");
            return;
        }

        var goal = new Goal
        {
            Title = title,
            TargetAmount = amount,
            Deadline = TargetDatePicker.Date
        };

        //_viewModel.AddGoalAsync(goal);
        await Navigation.PopAsync();
    }
}
