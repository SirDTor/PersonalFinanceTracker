﻿using PersonalFinanceTracker.Services;
using System.Globalization;

namespace PersonalFinanceTracker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        public static object DatabaseService { get; internal set; }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Page startPage = PinService.IsPinSet ? new NavigationPage(new Views.PinUnlockPage()) : new AppShell();
            return new Window(startPage);
        }        
    }
}