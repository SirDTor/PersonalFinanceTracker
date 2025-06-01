using System;
using System.Collections.ObjectModel;
using PersonalFinanceTracker.Models;
using SQLite;
using Microsoft.Maui.Controls;
using PersonalFinanceTracker.Services;

namespace PersonalFinanceTracker.Views
{
    public partial class AddTransactionPage : ContentPage
    {
        private DatabaseService _dataService;

        public ObservableCollection<string> Categories { get; set; }
        public string SelectedCategory { get; set; }

        public AddTransactionPage()
        {
            InitializeComponent();
            _dataService = new DatabaseService(Path.Combine(FileSystem.AppDataDirectory, "financedb.db"));

            Categories = new ObservableCollection<string>
            {
                "Еда", "Транспорт", "Жилье", "Здоровье", "Подарки", "Такси", "Развлечения", "Зарплата"
            };

            BindingContext = this;
        }

        private async void OnAddTransactionClicked(object sender, EventArgs e)
        {
            if (decimal.TryParse(AmountEntry.Text, out decimal amount) && !string.IsNullOrWhiteSpace(SelectedCategory))
            {
                var transaction = new Transaction
                {
                    Amount = amount,
                    Category = SelectedCategory,
                    Description = DescriptionEntry.Text,
                    Date = TransactionDatePicker.Date
                };

                _dataService.Add(transaction);
                await DisplayAlert("Готово", "Операция добавлена", "ОК");
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                await DisplayAlert("Ошибка", "Проверьте сумму и категорию", "ОК");
            }
        }
    }
}
