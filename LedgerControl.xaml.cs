using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

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
        }
    }
}
