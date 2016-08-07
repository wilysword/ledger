using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Budget
{
    class EntryCollection : ObservableCollection<Entry>
    {
        private readonly Account[] accounts;

        public EntryCollection(Account[] accounts, IEnumerable<Entry> initial)
            : base(initial)
        {
            this.accounts = accounts;
            Add(new Entry());
        }

        public Entry Reposition(int fromIndex)
        {
            if (fromIndex == 0)
                throw new NotImplementedException("Cannot reposition the first element at this time, due to the differing nature of sum calculations.");
            Entry entry = this[fromIndex];
            int newIndex = -1;
            for (int i = fromIndex - 1; i >= 0; i--)
            {
                if (entry.Date >= this[i].Date)
                    break;
                newIndex = i;
            }
            for (int i = fromIndex + 1; i < Count; i++)
            {
                if (this[i].Date == default(DateTime) || entry.Date <= this[i].Date)
                    break;
                newIndex = i;
            }
            if (newIndex == 0)
                throw new NotImplementedException("Cannot reposition the first element at this time, due to the differing nature of sum calculations.");
            int lastIndex = Count - 1;
            if (newIndex >= 0)
            {
                if (0 < fromIndex)
                    entry.BoundAmounts.Disconnect(this[fromIndex - 1]);
                if (fromIndex < lastIndex)
                    this[fromIndex + 1].BoundAmounts.Disconnect(entry);
                if (0 < fromIndex && fromIndex < lastIndex)
                    this[fromIndex + 1].BoundAmounts.Connect(this[fromIndex - 1]);
                // Note that removing first then inserting should work for both shifting up and shifting down:
                // UP: Removal will shift all the indices above the removal point, but that's good because
                //     it will insert the entry just above the last value with a Date less than this entry's.
                // DOWN: Removal will not affect anything at the new insertion point.
                RemoveAt(fromIndex);
                Insert(newIndex, entry);
                if (0 < newIndex && newIndex < lastIndex)
                    this[newIndex + 1].BoundAmounts.Disconnect(this[newIndex - 1]);
                if (newIndex < lastIndex)
                    this[newIndex + 1].BoundAmounts.Connect(entry);
                if (0 < newIndex)
                    entry.BoundAmounts.Connect(this[newIndex - 1]);
            }
            return AddBlank();
        }

        public Entry AddBlank()
        {
            var last = this.LastOrDefault();
            if (last == null || last.Date != default(DateTime))
            {
                var entry = new Entry();
                entry.Setup(accounts, last);
                Add(entry);
                return entry;
            }
            return null;
        }
    }

    /// <summary>
    /// Interaction logic for LedgerControl.xaml
    /// </summary>
    public partial class LedgerControl : UserControl
    {
        public LedgerControl()
        {
            InitializeComponent();
            fromDate(DateTime.Today.AddDays(-90));
        }

        private void fromDate(DateTime start)
        {
            var accounts = (from x in App.DB.Accounts
                            where x.EndDate == null || x.EndDate >= start
                            orderby x.Position, x.EndDate descending
                            select x).ToArray();
            var initialCounts = (from x in App.DB.EntryAmounts
                                 where x.Entry.Date < start
                                 group x by x.Account into grp
                                 select new { key = grp.Key, value = grp.Sum(y => y.Amount) })
                                 .ToDictionary(x => x.key, y => y.value);
            decimal initialCashBalance = (from c in initialCounts
                                          where c.Key.IsVirtual
                                          select c.Value).Sum();
            var entries = new EntryCollection(accounts, (from x in App.DB.Entries
                                                         where x.Date >= start
                                                         orderby x.Date
                                                         select x).Include(x => x.Amounts));

            entries[0].Setup(accounts, null);
            entries[0].PropertyChanged += entryDateChanged;
            foreach (var account in accounts)
            {
                if (initialCounts.ContainsKey(account))
                    entries[0].BoundAmounts[account].Sum = initialCounts[account];
                addColumn(account);
            }
            for (int i = 1; i < entries.Count; i++)
            {
                entries[i].Setup(accounts, entries[i - 1]);
                entries[i].PropertyChanged += entryDateChanged;
            }

            ledger.ItemsSource = entries;
            ledger.ScrollIntoView(entries.Last());
        }

        private void entryDateChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Entry entry = (Entry)sender;
            if (entry.Date == default(DateTime))
                return;
            if (entry.ID == 0)
                App.DB.Entries.Add(entry);
            var entries = ledger.ItemsSource as EntryCollection;
            try
            {
                var newEntry = entries.Reposition(ledger.SelectedIndex);
                if (newEntry != null)
                    newEntry.PropertyChanged += entryDateChanged;
            }
            catch (NotImplementedException) { refresh(); }
        }

        private void refresh()
        {
            App.RefreshDB();
            for (int i = grid.Columns.Count - 1; i > 1; i--)
                grid.Columns.RemoveAt(i);
            fromDate(DateTime.Today.AddDays(-90));
        }

        private void addColumn(Account account)
        {
            DataTemplate template = new DataTemplate();
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(EntryAmountControl));
            factory.SetBinding(EntryAmountControl.DataContextProperty, new Binding("BoundAmounts") { Converter = new AmountConverter(account) });
            template.VisualTree = factory;
            grid.Columns.Add(new GridViewColumn() { CellTemplate = template, Header = account.Name });
        }

        private void ledger_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete && e.OriginalSource is ListViewItem)
            {
                App.DB.Entries.Remove((Entry)ledger.SelectedItem);
                var entries = (EntryCollection)ledger.ItemsSource;
                entries.RemoveAt(ledger.SelectedIndex);
                entries.AddBlank();
            }
        }
    }
}
