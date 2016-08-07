using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Budget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            if (App.DB.Categories.Count() == 0)
                setupTestData();
            InitializeComponent();
        }

        private void setupTestData()
        {
            var acc1 = new Account() { Name = "Spending", Position = 2, IsVirtual = true };
            var acc2 = new Account() { Name = "Tithing", Position = 1, IsVirtual = true };
            var acc3 = new Account() { Name = "Checking", Position = 4 };
            var cat1 = new Category() { Name = "Tithing", Position = 10 };
            var entry1 = new Entry() { Check = true, Date = new DateTime(2016, 7, 31), Description = "Tithes & Offerings", Total = -70 };
            entry1.Amounts.Add(new EntryAmount() { Account = acc2, Amount = -70 });
            entry1.Amounts.Add(new EntryAmount() { Account = acc3, Amount = -100 });
            cat1.Entries.Add(entry1);
            cat1.Percentages.Add(new Percentage() { Account = acc2, Percent = 100 });
            var cat2 = new Category() { Name = "Entertainment", Position = 1 };
            cat2.Percentages.Add(new Percentage() { Account = acc1, Percent = 100 });
            cat2.Percentages.Add(new Percentage() { Account = acc3, Percent = 100 });
            var entry2 = new Entry() { Date = new DateTime(2016, 8, 2), Description = "Mission Impossible", Total = -24.5M };
            entry2.Amounts.Add(new EntryAmount() { Account = acc1, Amount = -24.5M });
            entry2.Amounts.Add(new EntryAmount() { Account = acc3, Amount = -24.5M });
            cat2.Entries.Add(entry2);
            var entry3 = new Entry() { Date = new DateTime(2016, 7, 2), Description = "Netflix", Total = -8.46M };
            entry3.Amounts.Add(new EntryAmount() { Account = acc3, Amount = -8.46M });
            cat2.Entries.Add(entry3);
            var entry4 = new Entry() { Date = new DateTime(2016, 4, 2), Description = "Initial", Total = 100 };
            entry4.Amounts.Add(new EntryAmount() { Account = acc3, Amount = 100 });
            cat2.Entries.Add(entry4);
            App.DB.Categories.Add(cat1);
            App.DB.Categories.Add(cat2);
            App.DB.SaveChanges();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
                App.DB.SaveChanges();
        }
    }
}
