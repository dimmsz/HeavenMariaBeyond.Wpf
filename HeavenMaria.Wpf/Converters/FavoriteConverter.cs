using HeavenMaria.Domain.Entities.Dtos;
using HeavenMaria.Domain.Entities.Main;
using System;
using System.Globalization;
using System.Windows.Data;

namespace HeavenMaria.Wpf.Converters
{
    public class FavoriteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var girl = value as GirlWithStatusDto;
            bool isFavorite = girl != null && girl.Favorite;

            if (isFavorite)
            {
                return "🧡";
            }
            else
            {
                return "💓";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
