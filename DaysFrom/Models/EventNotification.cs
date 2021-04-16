using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DaysFrom.Models
{
    public class EventNotification
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int EventId { get; set; }
        public int MonthQuantifier { get; set; }

    }
}
