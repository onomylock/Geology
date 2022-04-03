using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Geology.Utilities
{
    public class FloatingPointValueConverter : IValueConverter
    {       
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double v = (double)value;
            return v < 1e-3 || v >= 1000 ? v.ToString("E3") : v.ToString("0.###");
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return double.Parse(value as String);
        }

        public static String ConvertToString(double value)
        {
            return value < 1e-3 || value >= 1000 ? value.ToString("E3") : value.ToString("0.###");
        }
        public static String ConvertToString(float value)
        {
            return value < 1e-3 || value >= 1000 ? value.ToString("E3") : value.ToString("0.###");
        }
    }
}
