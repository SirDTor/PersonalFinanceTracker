using Microcharts;
using Microcharts.Maui;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using PersonalFinanceTracker.ViewModels;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Globalization;

namespace PersonalFinanceTracker.Views
{
    public partial class MainPage : ContentPage
    {
        private MainViewModel _viewModel;

        private DatabaseService _databaseService;

        public MainPage()
        {
            InitializeComponent();

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "financedb.db");
            _databaseService = new DatabaseService(dbPath);

            _viewModel = new MainViewModel();
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadData(); // Обновление при возврате на экран
        }

        private async void OnAddTransactionClicked(object sender, EventArgs e)
        {
            var page = new AddTransactionPage(_databaseService);
            await Shell.Current.Navigation.PushAsync(page);
        }

        private async void OnHistoryClicked(object sender, EventArgs e)
        {
            var page = new TransactionListPage(_databaseService);
            await Shell.Current.Navigation.PushAsync(page);
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("SettingsPage");
        }

        private async void OnViewGoalsClicked(object sender, EventArgs e)
        {
            var page = new GoalsPage(_databaseService);
            await Shell.Current.Navigation.PushAsync(page);
        }

        private async void OnViewBudgetsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Бюджет", "Раздел бюджета пока в разработке", "OK");
        }

        private async void OnAddGoalTapped(object sender, EventArgs e)
        {
            var goalViewModel = new GoalViewModel(_databaseService); // Создаём ViewModel с БД
            var page = new AddGoalPage(goalViewModel);
            await Shell.Current.Navigation.PushAsync(page);
        }
    }

}