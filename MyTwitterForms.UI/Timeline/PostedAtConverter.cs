using System;
using System.Globalization;
using Xamarin.Forms;

namespace MyTwitterForms.UI.Timeline
{
    //  ツイートの投稿日時 (DateTime) を文字列 (string) に変換する。
    internal class PostedAtConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (value is DateTime postedAt) ? postedAt.ToString("yyyy/MM/dd HH:mm:ss") : "-";

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
