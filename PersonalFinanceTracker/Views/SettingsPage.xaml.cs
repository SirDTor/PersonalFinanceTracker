using System;
using Microsoft.Maui.Controls;
using PersonalFinanceTracker.Services;

namespace PersonalFinanceTracker.Views
{
    public partial class SettingsPage : ContentPage
    {
        private readonly DatabaseService _dataService;

        public SettingsPage()
        {
            InitializeComponent();
            _dataService = new DatabaseService(Path.Combine(FileSystem.AppDataDirectory, "financedb.db"));
        }

        private async void OnExportNavigateClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ExportPage());
        }

        private async void OnDeleteAllClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("�������� ������", "������� ��� ����������?", "��", "������");
            if (!confirm) return;

            var all = _dataService.GetAll();
            foreach (var t in all)
                _dataService.Delete(t);

            StatusLabel.Text = "��� ������ �������.";
        }

        private async void OnPinSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PinSettingsPage());
        }


        // (�����������) ������ ����
        /*
        private void OnThemeSwitchChanged(object sender, ToggledEventArgs e)
        {
            Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
        }
        */
    }
}
