using DaysFrom.Services;
using Shiny.Jobs;
using Shiny.Notifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using NodaTime;
using System.Linq;

namespace DaysFrom.Jobs
{
    public class NotificationJob : IJob
    {
        readonly int[] hardCodedAnniversaries = new int[] { 1, 3, 6 };
        readonly Shiny.Notifications.INotificationManager _notificationManager;
        public NotificationJob(Shiny.Notifications.INotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }
        public async Task<bool> Run(JobInfo jobInfo, CancellationToken cancellationToken)
        {
            //await _notificationManager.Send("Background job ran", $"{DateTime.Now} - Last run time # {jobInfo.LastRunUtc}");
            var runJob = true;
            //TODO: try to run once a day
            if (runJob) {
                await CheckEventNotifications();
            }
            return false;
        }

        async Task CheckEventNotifications()
        {
            var events = await EventDataService.GetEvents();
            foreach(var eventModel in events)
            {
                var eventNotifications = await EventNotificationDataService.GetEventNotificationsById(eventModel.Id);
                var timeElapsed = Period.Between(LocalDateTime.FromDateTime(eventModel.EventDate), LocalDateTime.FromDateTime(DateTime.Now));
                var months = timeElapsed.Years * 12 + timeElapsed.Months;

                //check if months equals any of monthly anniversaries or is a yearly anniv
                if (hardCodedAnniversaries.Contains(months) || (months != 0 && months % 12 == 0))
                {
                    var hasBeenSent = eventNotifications.Where(x => x.MonthQuantifier == months);
                    
                    //no notification has been sent
                    if(hasBeenSent.Count() < 1)
                    {
                        await _notificationManager.Send("Congratulations!", $"It has been {months} months for {eventModel.Name}!");
                        await EventNotificationDataService.AddEventNotification(new Models.EventNotification { EventId = eventModel.Id, MonthQuantifier = months });
                    }
                }
            }
        }
    }
}
