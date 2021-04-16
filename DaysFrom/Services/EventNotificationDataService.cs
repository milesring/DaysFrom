using DaysFrom.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DaysFrom.Services
{
    public static class EventNotificationDataService
    {

        static SQLiteAsyncConnection db;

        static async Task Init()
        {
            if (db != null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "EventData.db");
            db = new SQLiteAsyncConnection(databasePath);
            await db.CreateTableAsync<EventNotification>();
        }

        public static async Task AddEventNotification(EventNotification eventNotification)
        {
            await Init();
            if (eventNotification.Id != 0)
            {
                await db.UpdateAsync(eventNotification);
            }
            else
            {
                await db.InsertAsync(eventNotification);
            }
        }

        public static async Task RemoveEventNotificationByEventId(int id)
        {
            await Init();
            await db.Table<EventNotification>().DeleteAsync(x => x.EventId == id);
        }

        public static async Task<IEnumerable<EventNotification>> GetEventNotificationsById(int id)
        {
            await Init();
            var events = await db.Table<EventNotification>().Where(x=>x.EventId == id).ToListAsync();
            return events;
        }
    }
}
