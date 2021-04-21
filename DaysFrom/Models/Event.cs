using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DaysFrom.Models
{
    public class Event
    {
        //TODO: add end date for static events
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public bool Favorite { get; set; }
        public DateTime EventCreation { get; set; }
    }
}
