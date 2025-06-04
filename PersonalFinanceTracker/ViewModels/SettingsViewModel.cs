using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using System.Threading.Tasks;
using PersonalFinanceTracker.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Windows;

namespace PersonalFinanceTracker.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly DatabaseService _dataService;

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand DeleteAllCommand { get; }
        public ICommand NavigateToExportCommand { get; }
        public ICommand NavigateToPinSettingsCommand { get; }

        public event Action NavigateToExportRequested;
        public event Action NavigateToPinSettingsRequested;

        public SettingsViewModel()
        {
            _dataService = new DatabaseService(Path.Combine(FileSystem.AppDataDirectory, "financedb.db"));

            DeleteAllCommand = new Command(async () => await DeleteAllAsync());
            NavigateToExportCommand = new Command(() => NavigateToExportRequested?.Invoke());
            NavigateToPinSettingsCommand = new Command(() => NavigateToPinSettingsRequested?.Invoke());
        }

        private async Task DeleteAllAsync()
        {
            bool confirmed = await Application.Current.MainPage.DisplayAlert("Удаление данных", "Удалить все транзакции?", "Да", "Отмена");
            if (!confirmed) return;

            var all = _dataService.GetAll();
            foreach (var t in all)
                _dataService.Delete(t);

            StatusMessage = "Все данные удалены.";
        }

        protected void SetProperty<T>(ref T backingField, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(backingField, value))
            {
                backingField = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
