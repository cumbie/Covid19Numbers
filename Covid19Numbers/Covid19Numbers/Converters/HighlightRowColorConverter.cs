using System;
using System.Globalization;
using Xamarin.Forms;

namespace Covid19Numbers.Converters
{
    public class HighlightRowColorConverter : IValueConverter
    {
        public HighlightRowColorConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Color))
            {
                string selected = (string)value;
                Color highlightColor = parameter != null ? (Color)parameter : Color.Transparent;
                return (selected == Settings.MyCountryCode) ? highlightColor : Color.Transparent;
            }
            else
                return Color.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
