using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Budget
{
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
            var entries = (from x in App.DB.Entries
                           where x.Date >= start
                           orderby x.Date
                           select x).Include(x => x.Amounts).ToList();
            var newEntry = new Entry();
            entries.Add(newEntry);

            entries[0].Setup(accounts, null);
            foreach (var account in accounts)
            {
                if (initialCounts.ContainsKey(account))
                    entries[0].BoundAmounts[account].Sum = initialCounts[account];
                addColumn(account);
            }
            for (int i = 1; i < entries.Count; i++)
                entries[i].Setup(accounts, entries[i - 1]);

            ledger.ItemsSource = entries;
            ledger.ScrollIntoView(newEntry);
        }

        private void addColumn(Account account)
        {
            DataTemplate template = new DataTemplate();
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(EntryAmountControl));
            factory.SetBinding(EntryAmountControl.DataContextProperty, new Binding("BoundAmounts") { Converter = new AmountConverter(account) });
            template.VisualTree = factory;
            grid.Columns.Add(new GridViewColumn() { CellTemplate = template, Header = account.Name });
        }
    }
}
