using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DaysFrom.Models
{
    public class Event
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public bool Favorite { get; set; }
        public DateTime EventCreation { get; set; }
        
    }
}
