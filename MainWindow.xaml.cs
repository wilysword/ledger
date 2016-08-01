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
            {
                App.DB.Categories.Add(new Budget.Category() { Name = "Tithing", Position = 10 });
                App.DB.Categories.Add(new Budget.Category() { Name = "Entertainment", Position = 1 });
                App.DB.SaveChanges();
            }
            InitializeComponent();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //tabControl.Width = e.NewSize.Width - 16;
            //tabControl.Height = e.NewSize.Height - 39;
        }
    }
}
