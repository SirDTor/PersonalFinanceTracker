using SQLite;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PersonalFinanceTracker.Models
{
    public enum TransactionType
    {
        Expense,
        Income
    }

    public class Transaction : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        TransactionType _type;
        public TransactionType Type
        {
            get => _type;
            set => SetField(ref _type, value, nameof(AmountDisplay), nameof(AmountColor));
        }

        string _category;
        public string Category
        {
            get => _category;
            set => SetField(ref _category, value);
        }

        decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set => SetField(ref _amount, value, nameof(AmountDisplay));
        }

        string? _description;
        public string? Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        DateTime _date;
        public DateTime Date
        {
            get => _date;
            set => SetField(ref _date, value);
        }

        public string AmountDisplay => $"{(Type == TransactionType.Income ? "+" : "-")}{Amount:C}";
        public string AmountColor => Type == TransactionType.Income ? "Green" : "Red";

        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        bool SetField<T>(ref T field, T value, params string[] extraProps)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged();
            foreach (var prop in extraProps)
                OnPropertyChanged(prop);
            return true;
        }
    }
}
