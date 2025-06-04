using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using PersonalFinanceTracker.Services;
using PersonalFinanceTracker.Views;

namespace PersonalFinanceTracker.ViewModels
{
    public class PinUnlockViewModel : INotifyPropertyChanged
    {
        private string _enteredPin;
        private string _statusMessage;

        public string EnteredPin
        {
            get => _enteredPin;
            set
            {
                _enteredPin = value;
                OnPropertyChanged();
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

        public ICommand UnlockCommand { get; }

        public PinUnlockViewModel()
        {
            UnlockCommand = new Command(Unlock);
        }

        private async void Unlock()
        {
            if (PinService.ValidatePin(EnteredPin))
            {
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                StatusMessage = "Неверный PIN-код";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
