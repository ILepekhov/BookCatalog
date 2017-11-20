using System;
using System.Globalization;
using System.Windows.Data;

namespace Shared.Binding
{
    [ValueConversion(typeof(string), typeof(string))]
    public sealed class StringTruncateConverter : IValueConverter
    {
        public int MaxStringLength { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var inputString = value.ToString();

            if (inputString.Length > MaxStringLength)
            {
                return inputString.Substring(0, MaxStringLength - 3) + "...";
            }

            return inputString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
