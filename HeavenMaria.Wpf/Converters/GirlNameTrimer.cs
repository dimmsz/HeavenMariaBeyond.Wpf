using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace HeavenMaria.Wpf.Converters
{
    public class GirlNameTrimer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string;

            text = text.Trim();
            text = text.Replace("\n", "");
            text = text.Replace("\r", "");
            text = text.Replace("\t", "");
            var result = Regex.Replace(text, " {6,}", "   ");
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
