using Proxy.BookSourceServer.GUI.ViewModel;
using System.Windows;

namespace Proxy.BookSourceServer.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) => DataContext = new ProxyViewModel();
        }
    }
}
