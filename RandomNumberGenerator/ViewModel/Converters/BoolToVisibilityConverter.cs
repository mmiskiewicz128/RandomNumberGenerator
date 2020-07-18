using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RandomNumberGenerator.ViewModel.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool oldValue = value as bool? ?? false;

            return oldValue ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility oldValue = value as Visibility? ?? Visibility.Hidden;

            return oldValue == Visibility.Visible ? true : false;
        }
    }
}
