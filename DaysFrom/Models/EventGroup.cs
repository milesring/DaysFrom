using System;
using System.Collections.Generic;
using System.Text;

namespace DaysFrom.Models
{
    public class EventGroup : List<Event>
    {
        public string Name { get; private set; }
        public EventGroup(string name, List<Event> events) : base(events)
        {
            Name = name;
        }
    }
}
