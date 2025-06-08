using CommunityToolkit.Mvvm.Input;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

public class GoalViewModel : INotifyPropertyChanged
{
    private readonly DatabaseService _db;
    public ObservableCollection<Goal> Goals { get; set; } = new();

    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand TopUpCommand { get; }

    public GoalViewModel(DatabaseService db)
    {
        _db = db;

        EditCommand = new AsyncRelayCommand<Goal>(EditGoalAsync);
        DeleteCommand = new RelayCommand<Goal>(DeleteGoal);
        TopUpCommand = new AsyncRelayCommand<Goal>(TopUpGoalAsync);

        LoadGoals();
    }

    private void LoadGoals()
    {
        Goals.Clear();
        foreach (var g in _db.GetGoals())
            Goals.Add(g);
    }

    private async Task EditGoalAsync(Goal goal)
    {
        if (goal == null) return;

        string newTitle = await Shell.Current.DisplayPromptAsync("Изменение", "Новое название:", initialValue: goal.Title);
        string newTarget = await Shell.Current.DisplayPromptAsync("Цель", "Новая сумма цели:", initialValue: goal.TargetAmount.ToString());
        string newDeadline = await Shell.Current.DisplayPromptAsync("Срок", "Новая дата (дд.мм.гггг):", initialValue: goal.Deadline.ToShortDateString());

        if (!string.IsNullOrWhiteSpace(newTitle) &&
            decimal.TryParse(newTarget, out var newAmount) &&
            DateTime.TryParse(newDeadline, out var newDate))
        {
            goal.Title = newTitle;
            goal.TargetAmount = newAmount;
            goal.Deadline = newDate;

            _db.UpdateGoal(goal);
            OnPropertyChanged(nameof(Goals));
        }
    }

    private void DeleteGoal(Goal goal)
    {
        if (goal == null) return;

        _db.DeleteGoal(goal);
        Goals.Remove(goal);
        OnPropertyChanged(nameof(Goals));
    }

    private async Task TopUpGoalAsync(Goal goal)
    {
        if (goal == null) return;

        string amountStr = await Shell.Current.DisplayPromptAsync("Пополнение", "Введите сумму:");
        if (decimal.TryParse(amountStr, out decimal addAmount))
        {
            goal.CurrentAmount += addAmount;
            _db.UpdateGoal(goal);
            OnPropertyChanged(nameof(Goals));
        }
    }

    public void AddGoal(Goal goal)
    {
        _db.AddGoal(goal);
        Goals.Add(goal);
        OnPropertyChanged(nameof(Goals));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
