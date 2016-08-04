using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Globalization;

namespace Budget
{
    public partial class EntryAmount
    {
        private decimal accumulatedSum;
        public decimal Sum
        {
            get { return accumulatedSum + Amount; }
        }
    }

    public partial class Entry
    {
        public readonly BalanceAmount Balance = new BalanceAmount() { Amount = 10, Sum = -5 };
    }

    public class BalanceAmount
    {
        public readonly Account Account = new Account() { Name = "Balance", IsVirtual = true };
        public decimal Amount { get; set; }
        public decimal Sum { get; set; }
    }

    public class AmountConverter : IValueConverter
    {
        private readonly Account account;
        public AmountConverter(Account account)
        {
            this.account = account;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Entry entry = value as Entry;
            EntryAmount amount = entry.Amounts.SingleOrDefault(x => x.AccountID == account.ID);
            if (amount == null)
                amount = new EntryAmount() { Amount = 4.5M, Account = account, Entry = entry };
            return amount;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interaction logic for EntryAmountControl.xaml
    /// </summary>
    public partial class EntryAmountControl : UserControl
    {
        public EntryAmountControl()
        {
            InitializeComponent();
        }

        private void updateSum(object sender, DataTransferEventArgs e)
        {
            var sumExpression = sum.GetBindingExpression(TextBlock.TextProperty);
            sumExpression.UpdateTarget();
        }

        private void amount_GotFocus(object sender, RoutedEventArgs e)
        {
            amount.SelectAll();
        }

        private void amount_LostFocus(object sender, RoutedEventArgs e)
        {
            // In order for this to do anything, I think I have to bind to a path on the Entry, instead of just the Entry itself.
            var binding = GetBindingExpression(EntryAmountControl.DataContextProperty);
            binding.UpdateSource();
        }
    }
}
