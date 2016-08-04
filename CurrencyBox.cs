using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Budget
{
    class CurrencyBox : TextBox
    {
        public string BindingPath
        {
            get { return GetBindingExpression(CurrencyBox.TextProperty).ParentBinding.Path.Path; }
            set
            {
                var textBinding = new Binding(value) { Converter = new CurrencyConverter() };
                textBinding.ValidationRules.Add(new CurrencyValidator());
                SetBinding(CurrencyBox.TextProperty, textBinding);
                SetBinding(CurrencyBox.ForegroundProperty, new Binding(value) { Converter = new CurrencyColorer() });
            }
        }

        public CurrencyBox()
        {
            TextAlignment = TextAlignment.Right;
            GotFocus += gotFocus;
        }

        private void gotFocus(object sender, RoutedEventArgs e)
        {
            SelectAll();
        }
    }
}
