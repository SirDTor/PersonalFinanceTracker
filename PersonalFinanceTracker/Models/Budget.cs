using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PersonalFinanceTracker.Models
{
    public class Budget : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        string _category = string.Empty;
        public string Category
        {
            get => _category;
            set => SetField(ref _category, value);
        }

        decimal _budgetAmount;
        public decimal BudgetAmount
        {
            get => _budgetAmount;
            set => SetField(ref _budgetAmount, value, nameof(RemainingAmount), nameof(SpentPercentage));
        }

        decimal _spentAmount;
        public decimal SpentAmount
        {
            get => _spentAmount;
            set => SetField(ref _spentAmount, value, nameof(RemainingAmount), nameof(SpentPercentage));
        }

        DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set => SetField(ref _startDate, value);
        }

        DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set => SetField(ref _endDate, value);
        }

        BudgetPeriod _period;
        public BudgetPeriod Period
        {
            get => _period;
            set => SetField(ref _period, value);
        }

        bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set => SetField(ref _isActive, value);
        }

        [Ignore]
        public decimal RemainingAmount => BudgetAmount - SpentAmount;

        [Ignore]
        public double SpentPercentage => BudgetAmount > 0 ? (double)(SpentAmount / BudgetAmount) : 0;

        [Ignore]
        public bool IsOverBudget => SpentAmount > BudgetAmount;

        [Ignore]
        public string StatusColor => IsOverBudget ? "Red" : SpentPercentage > 0.8 ? "Orange" : "Green";

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

    public enum BudgetPeriod
    {
        Weekly,
        Monthly,
        Quarterly,
        Yearly
    }
}
