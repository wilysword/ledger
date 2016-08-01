using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;

namespace Budget
{
    internal class CurrencyConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (decimal)value;
            if (val == default(decimal))
                return "";
            return val.ToString("C", culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as string;
            if (!string.IsNullOrEmpty(val))
                return decimal.Parse(val, NumberStyles.Currency, culture);
            return default(decimal);
        }
    }

    internal class CurrencyColorer : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((decimal)value < 0)
                return Brushes.Firebrick;
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class CurrencyValidator : ValidationRule
    {
        public bool AllowBlank { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var val = value as string;
            if (!string.IsNullOrEmpty(val))
                try { decimal.Parse(val, NumberStyles.Currency, cultureInfo); }
                catch (FormatException) { return new ValidationResult(false, "Invalid currency value."); }
            else if (!AllowBlank)
                return new ValidationResult(false, "Value cannot be blank.");
            return new ValidationResult(true, null);
        }
    }
}
