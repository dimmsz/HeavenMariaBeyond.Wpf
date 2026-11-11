using HeavenMariaBeyond.Domain.Entities.Dtos;
using System;
using System.Globalization;
using System.Windows.Data;

namespace HeavenMariaBeyond.Wpf.Converters
{
    public class LegendConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var girl = value as GirlWithStatusDto;
            bool isLegend = girl != null && girl.Legend;

            if (isLegend)
            {
                return "★";
            }
            else
            {
                return "☆";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
