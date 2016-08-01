using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Budget
{
    internal class NullDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dt = (DateTime)value;
            if (dt == default(DateTime))
                return null;
            return dt;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return default(DateTime);
            return value;
        }
    }

    /// <summary>
    /// Interaction logic for EntryControl.xaml
    /// </summary>
    public partial class EntryControl : UserControl
    {
        public EntryControl()
        {
            InitializeComponent();
            category.ItemsSource = App.DB.Categories.OrderBy(x => x.Position).ToArray();
        }

        private void total_GotFocus(object sender, RoutedEventArgs e)
        {
            total.SelectAll();
        }
    }
}
