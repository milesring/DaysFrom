using DaysFrom.Services;
using DaysFrom.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DaysFrom
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            DependencyService.Register<INotificationManager>();
            //DependencyService.Register<MockDataStore>();
            Current.UserAppTheme = Application.Current.RequestedTheme;
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
