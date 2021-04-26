using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DaysFrom.Converters
{
    public class InvertedDateTimeToVisibilityConverter : DateTimeToVisibilityConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (DateTime)value;
            if (date == DateTime.MinValue)
            {
                date = DateTime.Now;
            }
            else 
            {
                date = DateTime.MinValue;
            }
            return base.Convert(date as object, targetType, parameter, culture);
        }
    }
}
