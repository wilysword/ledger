using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Budget
{
    public partial class EntryAmount : INotifyPropertyChanged
    {
        // Note that when assigning to Sum, it's the value UP TO this entry,
        // whereas reading from Sum is the value INCLUDING this entry.
        private decimal sum;
        public decimal Sum
        {
            get { return sum + Amount; }
            set
            {
                if (sum != value)
                {
                    sum = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Sum"));
                }
            }
        }

        /// <summary>
        /// Wrapper around Amount that adds PropertyChanged invocations.
        /// </summary>
        public decimal BoundAmount
        {
            get { return Amount; }
            set
            {
                if (Amount != value)
                {
                    Amount = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BoundAmount"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Sum"));
                }
            }
        }

        public void Connect(EntryAmount previous)
        {
            sum = previous.Sum;
            previous.PropertyChanged += propagateSum;
        }

        private void propagateSum(object sender, PropertyChangedEventArgs e)
        {
            Sum = (sender as EntryAmount).Sum;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class EntryAmountCollection : Dictionary<Account, EntryAmount>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public readonly Entry Owner;

        public EntryAmountCollection(Entry owner, Account[] accounts) :
            base(owner.Amounts.ToDictionary(x => x.Account))
        {
            Owner = owner;
            foreach (var account in accounts)
            {
                if (!ContainsKey(account))
                    Add(account, new EntryAmount() { Entry = owner, Account = account });
            }
        }
    }

    public partial class Entry
    {
        public BalanceAmount Balance { get; private set; }

        public EntryAmountCollection BoundAmounts { get; private set; }

        public void Setup(Account[] accounts, Entry previousEntry)
        {
            Balance = new BalanceAmount() { BoundAmount = 10, Sum = -5 };
            BoundAmounts = new EntryAmountCollection(this, accounts);
            if (previousEntry != null)
                foreach (var account in accounts)
                    BoundAmounts[account].Connect(previousEntry.BoundAmounts[account]);
        }
    }

    public class BalanceAmount
    {
        public readonly Account Account = new Account() { Name = "Balance", IsVirtual = true };
        public decimal BoundAmount { get; set; }
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
            var amounts = value as EntryAmountCollection;
            return amounts[account];
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

        private void amount_LostFocus(object sender, RoutedEventArgs e)
        {
            // Here we need to tell the EntryAmountCollection to fix itself.
        }
    }
}
