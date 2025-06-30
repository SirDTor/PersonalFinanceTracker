using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using PersonalFinanceTracker.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PersonalFinanceTracker.ViewModels
{
    public partial class GoalViewModel : BaseViewModel
    {
        private readonly IDatabaseService _databaseService;

        [ObservableProperty]
        private ObservableCollection<Goal> _goals;

        [ObservableProperty]
        private Goal _selectedGoal;

        [ObservableProperty]
        private bool _isAddingGoal;

        [ObservableProperty]
        private string _goalName;

        [ObservableProperty]
        private decimal _goalAmount;

        public GoalViewModel(IDatabaseService databaseService) : base()
        {
            _databaseService = databaseService;
            LoadGoalsAsync();
        }

        private async Task LoadGoalsAsync()
        {
            var goals = await _databaseService.GetGoalsAsync();
            Goals = new ObservableCollection<Goal>(goals);
        }

        [RelayCommand]
        private async Task AddGoalAsync()
        {
            if (IsAddingGoal)
            {
                var goal = new Goal { Name = GoalName, TargetAmount = GoalAmount };
                _databaseService.AddGoal(goal);
                Goals.Add(goal);
                IsAddingGoal = false;
            }
            else
            {
                IsAddingGoal = true;
            }
        }

        [RelayCommand]
        private async Task EditGoalAsync()
        {
            if (SelectedGoal != null)
            {
                _databaseService.UpdateGoal(SelectedGoal);
            }
        }

        [RelayCommand]
        private async Task DeleteGoalAsync()
        {
            if (SelectedGoal != null)
            {
                _databaseService.DeleteGoal(SelectedGoal);
                Goals.Remove(SelectedGoal);
            }
        }
    }
}