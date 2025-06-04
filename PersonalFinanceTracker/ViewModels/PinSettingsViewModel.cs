using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using PersonalFinanceTracker.Services;

namespace PersonalFinanceTracker.ViewModels
{
    public class PinSettingsViewModel : INotifyPropertyChanged
    {
        private string _newPin;
        private string _statusMessage;

        public string NewPin
        {
            get => _newPin;
            set
            {
                if (_newPin != value)
                {
                    _newPin = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand SavePinCommand { get; }
        public ICommand RemovePinCommand { get; }

        public PinSettingsViewModel()
        {
            SavePinCommand = new Command(SavePin);
            RemovePinCommand = new Command(RemovePin);
        }

        private void SavePin()
        {
            if (!string.IsNullOrWhiteSpace(NewPin))
            {
                PinService.SetPin(NewPin);
                StatusMessage = "PIN-код сохранён.";
                NewPin = string.Empty;
            }
            else
            {
                StatusMessage = "Введите PIN-код.";
            }
        }

        private void RemovePin()
        {
            PinService.RemovePin();
            StatusMessage = "PIN-код удалён.";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
