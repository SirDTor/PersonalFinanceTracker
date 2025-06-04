using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using Microsoft.Maui.Controls;

namespace PersonalFinanceTracker.ViewModels
{
    public class AddTransactionViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> Categories { get; set; } = new ObservableCollection<string>
        {
            "Еда", "Транспорт", "Развлечения", "Жилье", "Поступление средств", "Перевод средств","Прочее"
        };

        public ObservableCollection<TransactionType> TransactionTypes { get; } = new()
        {
            TransactionType.Expense,
            TransactionType.Income
        };

        private string _selectedCategory;       

        public string SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        private DateTime _date = DateTime.Now;
        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        private TransactionType _selectedTransactionType;

        public TransactionType SelectedTransactionType
        {
            get => _selectedTransactionType;
            set => SetProperty(ref _selectedTransactionType, value);
        }

        public ICommand AddTransactionCommand { get; }

        public AddTransactionViewModel(DatabaseService db)
        {
            _databaseService = db;
            AddTransactionCommand = new Command(OnAddTransaction);
        }

        private async void OnAddTransaction()
        {
            if (Amount != 0 && !string.IsNullOrWhiteSpace(SelectedCategory))
            {
                var transaction = new Transaction
                {
                    Amount = Amount,
                    Category = SelectedCategory,
                    Description = Description,
                    Date = Date,
                    Type = SelectedTransactionType
                };
                _databaseService.Add(transaction);
                await Shell.Current.DisplayAlert("Готово", "Операция добавлена", "ОК");
                await Shell.Current.GoToAsync(".."); // Возврат назад
            }
            else
            {
                await Shell.Current.DisplayAlert("Ошибка", "Проверьте сумму и категорию", "ОК");
            }
        }

        protected void SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propName = null)
        {
            if (!Equals(backingField, value))
            {
                backingField = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
