using NodaTime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace DaysFrom.Converters
{
    public class DateTimeToDaysSinceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //TODO: Convert to NodaTime values
            var date = LocalDateTime.FromDateTime((DateTime)value);
            var timeSince = Period.Between(date, LocalDateTime.FromDateTime(DateTime.Now));
            var days = timeSince.Days;
            var hours = timeSince.Hours;
            var min = timeSince.Minutes;

            string tense = "have passed.";
            if(timeSince.Ticks < 0)
            {
                tense = "until.";
            }

            string yearsString = "";
            if (timeSince.Years != 0)
            {
                yearsString = $"{Math.Abs(timeSince.Years)} years, ";
            }

            string monthsString = "";
            if(timeSince.Months != 0)
            {
                monthsString = $"{Math.Abs(timeSince.Months)} months, ";
            }
            string daysString = "";
            if(days != 0)
            {
                daysString = $"{Math.Abs(days)} days, ";
            }
            string hoursString = "";
            if(hours != 0)
            {
                hoursString = $"{Math.Abs(hours)} hours, ";
            }

            return $"{yearsString}{monthsString}{daysString}{hoursString}{Math.Abs(min)} minutes {tense}"; ;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
