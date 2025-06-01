using Microcharts;
using Microcharts.Maui;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using SkiaSharp;
using System.Globalization;

namespace PersonalFinanceTracker.Views
{
    public partial class MainPage : ContentPage
    {
        private DatabaseService _dataService;

        public MainPage()
        {
            InitializeComponent();
            _dataService = new DatabaseService(Path.Combine(FileSystem.AppDataDirectory, "financedb.db"));
            LoadData();
        }

        private void LoadData()
        {
            var transactions = _dataService.GetAll();
            var entries = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .GroupBy(t => t.Category)
                .Select(g => new ChartEntry((float)g.Sum(t => t.Amount))
                {
                    Label = g.Key,
                    ValueLabel = g.Sum(t => t.Amount).ToString("F0") + "₽",
                    Color = SKColor.Parse("#4CAF50")
                }).ToList();

            ChartView.Chart = new DonutChart 
            { 
                Entries = entries, 
                LabelMode = LabelMode.RightOnly,
                LabelTextSize = 30,                
                BackgroundColor = SKColor.Parse ("#E6F2EE"),
                AnimationProgress = 5
            };

            BalanceLabel.Text = _dataService.GetBalance().ToString("C0", CultureInfo.CreateSpecificCulture("ru-RU"));
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