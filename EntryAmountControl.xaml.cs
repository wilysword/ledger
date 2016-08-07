using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
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
            Sum = previous.Sum;
            previous.PropertyChanged += propagateSum;
        }

        public void Disconnect(EntryAmount previous)
        {
            previous.PropertyChanged -= propagateSum;
        }

        private void propagateSum(object sender, PropertyChangedEventArgs e)
        {
            Sum = (sender as EntryAmount).Sum;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class EntryAmountCollection : Dictionary<Account, EntryAmount>
    {
        public readonly Entry Owner;

        public EntryAmountCollection(Entry owner, Account[] accounts) :
            base(owner.Amounts.ToDictionary(x => x.Account))
        {
            Owner = owner;
            foreach (var account in accounts)
            {
                if (!ContainsKey(account))
                    Add(account, new EntryAmount() { Entry = owner, Account = account });
                this[account].PropertyChanged += syncToOwner;
            }
        }

        public void Connect(Entry previous)
        {
            foreach (var amount in Values)
                amount.Connect(previous.BoundAmounts[amount.Account]);
        }

        public void Disconnect(Entry previous)
        {
            foreach (var amount in Values)
                amount.Disconnect(previous.BoundAmounts[amount.Account]);
        }

        private void syncToOwner(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BoundAmount")
            {
                EntryAmount amount = sender as EntryAmount;
                bool isOwned = Owner.Amounts.Contains(amount);
                if (amount.Amount == 0 && isOwned)
                    Owner.Amounts.Remove(amount);
                else if (amount.Amount != 0 && !isOwned)
                    Owner.Amounts.Add(amount);
            }
        }
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }

    public class AmountEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is EntryAmount;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
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
    }
}
