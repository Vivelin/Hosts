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
using Vivelin.Hosts;

namespace Hosts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HostsFileManager HostsFileManager
        {
            get { return (HostsFileManager)DataContext; }
            set { DataContext = value; }
        }

        public MainWindow()
        {
            InitializeComponent();

            HostsFileManager = new HostsFileManager();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await HostsFileManager.LoadAsync().ConfigureAwait(false);
        }
    }
}
