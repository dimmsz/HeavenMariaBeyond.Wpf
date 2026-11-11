using HeavenMariaBeyond.Domain.Entities.Main;
using System;
using System.Globalization;
using System.Windows.Data;

namespace HeavenMariaBeyond.Wpf.Converters
{
    public class DiaryTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string diaryTitle = string.Empty;
            if (value is Diary diary)
            {
                string dateTimeString = diary.DiaryDate.ToString("yyyy年MM月dd日");
                diaryTitle = $"{dateTimeString}  {diary.DiaryTitle} ";

                if (diary.MovieFrameCompleted)
                {
                    diaryTitle = "🎦 " + diaryTitle;
                }
            }
            return diaryTitle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
