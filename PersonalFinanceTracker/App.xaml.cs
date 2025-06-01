using PersonalFinanceTracker.Services;
using System.Globalization;

namespace PersonalFinanceTracker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Проверяем, установлен ли PIN
            Page startPage;

            if (PinService.IsPinSet)
            {
                // PIN установлен — сначала открываем PinUnlockPage
                startPage = new NavigationPage(new Views.PinUnlockPage());
            }
            else
            {
                // Если PIN не задан — сразу основная оболочка
                startPage = new AppShell();
            }

            return new Window(startPage);
        }        
    }
}