using System.Collections.Generic;
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
            var cat = App.DB.Categories.First();
            ledger.ItemsSource = new List<Entry>() { new Entry(), new Entry() { Category = cat, Check = true, Description = "Hey", Total = -5 } };
            ledger.ScrollIntoView(14);

            addColumn(new Account() { Name = "Awesome" });
            addColumn(new Account() { Name = "Incredible" });
        }

        private void addColumn(Account account)
        {
            DataTemplate template = new DataTemplate();
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(EntryAmountControl));
            factory.SetBinding(EntryAmountControl.DataContextProperty, new Binding() { Converter = new AmountConverter(account) });
            template.VisualTree = factory;
            grid.Columns.Add(new GridViewColumn() { CellTemplate = template, Header = account.Name });
        }
    }
}
