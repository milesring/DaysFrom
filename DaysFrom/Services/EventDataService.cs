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
    public static class EventDataService
    {
        static SQLiteAsyncConnection db;

        static async Task Init()
        {
            if (db != null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "EventData.db");
            db = new SQLiteAsyncConnection(databasePath);
            await db.CreateTableAsync<Event>();
        }

        public static async Task AddEvent(Event eventModel)
        {
            await Init();
            if(eventModel.Id != 0)
            {
                await db.UpdateAsync(eventModel);
            }
            else
            {
                await db.InsertAsync(eventModel);
            }
            

        }

        public static async Task RemoveEvent(int id)
        {
            await Init();
            await db.DeleteAsync<Event>(id);
        }

        public static async Task<IEnumerable<Event>> GetEvent()
        {
            await Init();
            var events = await db.Table<Event>().ToListAsync();
            return events;
        }
    }
}
