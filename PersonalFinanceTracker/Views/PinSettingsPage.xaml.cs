using Microsoft.Maui.Controls;
using PersonalFinanceTracker.Services;

namespace PersonalFinanceTracker.Views
{
    public partial class PinSettingsPage : ContentPage
    {
        public PinSettingsPage()
        {
            InitializeComponent();
        }

        private void OnSavePinClicked(object sender, EventArgs e)
        {
            var pin = PinEntry.Text?.Trim();
            if (string.IsNullOrWhiteSpace(pin) || pin.Length < 4)
            {
                StatusLabel.Text = "PIN должен содержать минимум 4 цифры.";
                return;
            }

            PinService.SetPin(pin);
            StatusLabel.Text = "PIN установлен.";
        }

        private void OnRemovePinClicked(object sender, EventArgs e)
        {
            if (PinService.IsPinSet)
            {
                PinService.RemovePin();
                StatusLabel.Text = "PIN удалён.";
            }
            else
            {
                StatusLabel.Text = "PIN не был установлен.";
            }
        }
    }
}
