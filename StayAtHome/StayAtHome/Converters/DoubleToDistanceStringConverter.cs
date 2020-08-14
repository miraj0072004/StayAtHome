using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace StayAtHome.Converters
{
    public class DoubleToDistanceStringConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValue = (double) value;

            if (doubleValue < 1000)
            {
                return Math.Round(doubleValue) + " m";
            }

            return Math.Round(doubleValue/1000,2) + " km";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
