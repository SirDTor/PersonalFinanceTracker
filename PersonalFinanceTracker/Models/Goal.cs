using SQLite;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PersonalFinanceTracker.Models
{
    public class Goal : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }

        // Add Name property as alias for Title for compatibility with analytics
        [Ignore]
        public string Name
        {
            get => Title;
            set => Title = value;
        }

        decimal _targetAmount;
        public decimal TargetAmount
        {
            get => _targetAmount;
            set
            {
                if (SetField(ref _targetAmount, value))
                    OnPropertyChanged(nameof(Progress)); // Обновляем Progress при изменении цели
            }
        }

        decimal _currentAmount;
        public decimal CurrentAmount
        {
            get => _currentAmount;
            set
            {
                if (SetField(ref _currentAmount, value))
                    OnPropertyChanged(nameof(Progress)); // Обновляем Progress при изменении текущей суммы
            }
        }

        DateTime _deadline;
        public DateTime Deadline
        {
            get => _deadline;
            set => SetField(ref _deadline, value);
        }

        // Add TargetDate property as alias for Deadline for compatibility with analytics
        [Ignore]
        public DateTime TargetDate
        {
            get => Deadline;
            set => Deadline = value;
        }

        [Ignore]
        // Новое вычисляемое свойство для прогресса (0..1)
        public double Progress
        {
            get => TargetAmount > 0 ? (double)(CurrentAmount / TargetAmount) : 0;
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
