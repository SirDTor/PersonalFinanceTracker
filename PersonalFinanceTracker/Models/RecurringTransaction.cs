using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PersonalFinanceTracker.Models
{
    public class RecurringTransaction : INotifyPropertyChanged
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

        TransactionType _type;
        public TransactionType Type
        {
            get => _type;
            set => SetField(ref _type, value);
        }

        string _category = string.Empty;
        public string Category
        {
            get => _category;
            set => SetField(ref _category, value);
        }

        decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set => SetField(ref _amount, value);
        }

        string? _description;
        public string? Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        RecurrenceFrequency _frequency;
        public RecurrenceFrequency Frequency
        {
            get => _frequency;
            set => SetField(ref _frequency, value);
        }

        DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set => SetField(ref _startDate, value, nameof(NextDueDate));
        }

        DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set => SetField(ref _endDate, value);
        }

        DateTime _lastExecuted;
        public DateTime LastExecuted
        {
            get => _lastExecuted;
            set => SetField(ref _lastExecuted, value, nameof(NextDueDate));
        }

        bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set => SetField(ref _isActive, value);
        }

        [Ignore]
        public DateTime NextDueDate
        {
            get
            {
                var baseDate = LastExecuted == default ? StartDate : LastExecuted;
                return Frequency switch
                {
                    RecurrenceFrequency.Daily => baseDate.AddDays(1),
                    RecurrenceFrequency.Weekly => baseDate.AddDays(7),
                    RecurrenceFrequency.BiWeekly => baseDate.AddDays(14),
                    RecurrenceFrequency.Monthly => baseDate.AddMonths(1),
                    RecurrenceFrequency.Quarterly => baseDate.AddMonths(3),
                    RecurrenceFrequency.Yearly => baseDate.AddYears(1),
                    _ => baseDate
                };
            }
        }

        [Ignore]
        public bool IsDue => NextDueDate <= DateTime.Now && IsActive;

        [Ignore]
        public bool IsOverdue => NextDueDate < DateTime.Now.Date && IsActive;

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

    public enum RecurrenceFrequency
    {
        Daily,
        Weekly,
        BiWeekly,
        Monthly,
        Quarterly,
        Yearly
    }
}
