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
            var date = (DateTime)value;
            var timeSince = DateTime.Now - date;
            var days = timeSince.Days;
            var hours = timeSince.Hours;
            var min = timeSince.Minutes;

            string tense = "have passed.";
            if(days < 0)
            {
                days *= -1;
                hours *= -1;
                min *= -1;
                tense = "until.";
            }

            string yearsString = "";
            if (days > 365)
            {
                var years = days / 365;
                days = days % 365;
                yearsString = $"{years} years, ";
            }

            return $"{yearsString}{days} days, {hours} hours, and {min} minutes {tense}"; ;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
