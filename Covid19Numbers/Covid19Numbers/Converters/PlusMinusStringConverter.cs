using System;
using System.Globalization;
using Xamarin.Forms;

namespace Covid19Numbers.Converters
{
    public class PlusMinusStringConverter : IValueConverter
    {
        public PlusMinusStringConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(string))
            {
                string retStr = "";
                if (value is int)
                {
                    int val = (int)value;

                    retStr = (val > 0) ? $"+{val}" : val.ToString();
                }
                if (value is bool)
                {
                    bool up = (bool)value;
                    //retStr += up ? " \u2191" : " \u2193";
                    retStr += up ? " \u25B2" : " \u25BC";
                }

                return retStr;
            }
            else if (targetType == typeof(Color))
            {
                Color retColor = Color.Transparent;

                if (value is int)
                {
                    int val = (int)value;  // up: \u2191 or \u25B2, down: \u2193 or \u25BC
                    retColor = (val > 0) ? Color.Red : (val < 0 ? Color.Green : Color.Black);
                }
                else if (value is bool)
                {
                    bool up = (bool)value;
                    retColor = up ? Color.Red : Color.Green;
                }

                return retColor;
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
