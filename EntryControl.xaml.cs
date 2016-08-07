using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Budget
{
    public partial class Entry : INotifyPropertyChanged
    {
        public BalanceAmount Balance { get; private set; }

        public EntryAmountCollection BoundAmounts { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Setup(Account[] accounts, Entry previousEntry)
        {
            Balance = new BalanceAmount() { BoundAmount = 10, Sum = -5 };
            BoundAmounts = new EntryAmountCollection(this, accounts);
            if (previousEntry != null)
                BoundAmounts.Connect(previousEntry);
        }

        public DateTime? BoundDate
        {
            get
            {
                if (Date == default(DateTime))
                    return null;
                return Date;
            }
            set
            {
                if (!value.HasValue)
                    value = default(DateTime);
                if (value != Date)
                {
                    Date = value.Value;
                    // Using "Date" instead of "BoundDate" because I don't want the data binding to loop unnecessarily.
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Date"));
                }
            }
        }
    }

    public class BalanceAmount
    {
        public decimal BoundAmount { get; set; }
        public decimal Sum { get; set; }
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
    }
}
