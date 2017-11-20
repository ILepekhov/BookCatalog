using GUI.Client.ViewModel;
using System.Windows;

namespace GUI.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                DataContext = new BookCatalogViewModel();
            };
        }
    }
}
