using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace StayAtHome.Converters
{
    public class DoubleToIconStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var journeyOngoing = (bool)value;

            if (journeyOngoing)
            {
                return "fa-stop";
            }

            return "fa-play";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
