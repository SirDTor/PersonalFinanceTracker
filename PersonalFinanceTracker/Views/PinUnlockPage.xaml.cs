using Microsoft.Maui.Controls;
using PersonalFinanceTracker.Services;

namespace PersonalFinanceTracker.Views
{
    public partial class PinUnlockPage : ContentPage
    {
        public PinUnlockPage()
        {
            InitializeComponent();
        }

        private async void OnUnlockClicked(object sender, EventArgs e)
        {
            if (PinService.ValidatePin(PinEntry.Text?.Trim()))
            {
                Application.Current.MainPage = new AppShell(); // переходим в главное окно
            }
            else
            {
                StatusLabel.Text = "Неверный PIN-код.";
            }
        }
    }
}
