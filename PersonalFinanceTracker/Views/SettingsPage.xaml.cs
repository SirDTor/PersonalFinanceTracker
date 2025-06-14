using ClosedXML.Excel;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.ViewModels;
using System.Globalization;

namespace PersonalFinanceTracker.Views
{
    public partial class SettingsPage : ContentPage
    {
        private SettingsViewModel ViewModel => (SettingsViewModel)BindingContext;

        public SettingsPage()
        {
            InitializeComponent();

            ViewModel.NavigateToExportRequested += async () =>
                await Navigation.PushAsync(new ExportPage());

            ViewModel.NavigateToPinSettingsRequested += async () =>
                await Navigation.PushAsync(new PinSettingsPage());
        }
    }
}
