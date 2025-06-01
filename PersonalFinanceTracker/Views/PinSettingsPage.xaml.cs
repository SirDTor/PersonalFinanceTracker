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
                StatusLabel.Text = "PIN ������ ��������� ������� 4 �����.";
                return;
            }

            PinService.SetPin(pin);
            StatusLabel.Text = "PIN ����������.";
        }

        private void OnRemovePinClicked(object sender, EventArgs e)
        {
            if (PinService.IsPinSet)
            {
                PinService.RemovePin();
                StatusLabel.Text = "PIN �����.";
            }
            else
            {
                StatusLabel.Text = "PIN �� ��� ����������.";
            }
        }
    }
}
