using DaysFrom.Jobs;
using DaysFrom.Services;
using DaysFrom.Views;
using Shiny;
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
            Current.UserAppTheme = Current.RequestedTheme;
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            var jobInfo = new Shiny.Jobs.JobInfo(typeof(NotificationJob), identifier: nameof(NotificationJob));
            jobInfo.PeriodicTime = new TimeSpan(24, 0, 0);
            await ShinyHost.Resolve<Shiny.Jobs.IJobManager>().Schedule(jobInfo);
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
