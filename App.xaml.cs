using System;
using System.Windows;

namespace Budget
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static ModelsContainer DB { get; private set; }

        static App()
        {
            DB = new ModelsContainer();
        }

        internal static void RefreshDB()
        {
            DB.SaveChanges();
            DB = new ModelsContainer();
        }
    }
}
