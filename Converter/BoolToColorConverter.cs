using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace FinanzasApp.Converters
{
    // true  => color para ingresos
    // false => color para gastos
    public class BoolToColorConverter : IValueConverter
    {
        public Color TrueColor { get; set; } = Color.FromArgb("#22c55e");   // verde
        public Color FalseColor { get; set; } = Color.FromArgb("#f97373"); // rojo

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? TrueColor : FalseColor;

            return FalseColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
