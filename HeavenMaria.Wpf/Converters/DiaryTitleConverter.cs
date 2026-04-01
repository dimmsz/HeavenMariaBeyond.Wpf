using HeavenMaria.Domain.Entities.Main;
using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using ZstdSharp;

namespace HeavenMaria.Wpf.Converters
{
    public class DiaryTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string diaryTitle = string.Empty;
            if (value is DiaryOld diary)
            {
                string dateTimeString = diary.DiaryDate.ToString("yyyy年MM月dd日");
                diaryTitle = $"{dateTimeString}  {DecompressToString(diary.DiaryTitle)} ";

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

        private string DecompressToString(byte[] compressed) => Encoding.UTF8.GetString(Decompress(compressed));

        private byte[] Decompress(byte[] compressedData)
        {
            // Decompressor のインスタンスを作成
            using (var decompressor = new Decompressor())
            {
                // Unwrap() メソッドで解凍を実行
                return decompressor.Unwrap(compressedData).ToArray();
            }
        }
    }
}
