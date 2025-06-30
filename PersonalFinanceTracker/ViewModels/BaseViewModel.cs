using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace PersonalFinanceTracker.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        [ObservableProperty]
        string title = string.Empty;

        [ObservableProperty]
        bool isRefreshing;

        public bool IsNotBusy => !IsBusy;

        [RelayCommand]
        protected virtual async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
