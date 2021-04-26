using DaysFrom.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NodaTime;
using Xamarin.Forms;

namespace DaysFrom.Converters
{
    public class DateSpanToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var eventModel = (Event)value;
            if (eventModel == null || eventModel.EventEndDate == DateTime.MinValue || eventModel.EventDate > DateTime.Now)
            {
                return null;
            }
            var tenseString = string.Empty;
            LocalDateTime endDate;
            if(DateTime.Now < eventModel.EventEndDate)
            {
                endDate = LocalDateTime.FromDateTime(DateTime.Now);
                tenseString = " so far";
            }
            else
            {
                endDate = LocalDateTime.FromDateTime(eventModel.EventEndDate);
            }
            var timeSpanned = Period.Between(LocalDateTime.FromDateTime(eventModel.EventDate), endDate);
            string timeSpannedString = $"{timeSpanned.Minutes} minutes";

            if (timeSpanned.Hours > 0)
            {
                timeSpannedString = timeSpannedString.Insert(0, $"{timeSpanned.Hours} hours, ");
            }

            if (timeSpanned.Days > 0)
            {
                timeSpannedString = timeSpannedString.Insert(0, $"{timeSpanned.Days} days, ");
            }
            if (timeSpanned.Months > 0)
            {
                timeSpannedString = timeSpannedString.Insert(0, $"{timeSpanned.Months} months, ");
            }
            if (timeSpanned.Years > 0)
            {
                timeSpannedString = timeSpannedString.Insert(0, $"{timeSpanned.Years} years, ");
            }

            return $"Lasted {timeSpannedString}{tenseString}.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
