using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PersonalFinanceTracker.Models
{
    public class Category : INotifyPropertyChanged
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

        string _icon = "💰";
        public string Icon
        {
            get => _icon;
            set => SetField(ref _icon, value);
        }

        string _color = "#512BD4";
        public string Color
        {
            get => _color;
            set => SetField(ref _color, value);
        }

        TransactionType _transactionType;
        public TransactionType TransactionType
        {
            get => _transactionType;
            set => SetField(ref _transactionType, value);
        }

        bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set => SetField(ref _isActive, value);
        }

        bool _isDefault = false;
        public bool IsDefault
        {
            get => _isDefault;
            set => SetField(ref _isDefault, value);
        }

        DateTime _createdAt = DateTime.Now;
        public DateTime CreatedAt
        {
            get => _createdAt;
            set => SetField(ref _createdAt, value);
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
