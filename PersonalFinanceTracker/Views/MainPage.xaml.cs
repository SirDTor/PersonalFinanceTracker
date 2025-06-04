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

        public MainPage()
        {
            InitializeComponent();

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "financedb.db");
            var dbService = new DatabaseService(dbPath);

            _viewModel = new MainViewModel(dbService);
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadTransactions(); // Обновление при возврате на экран
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTransactionPage());
        }

        private async void OnListClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TransactionListPage());
        }
    }

}