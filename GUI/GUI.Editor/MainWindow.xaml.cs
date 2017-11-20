using GUI.Editor.ViewModel;
using System.Windows;

namespace GUI.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) => DataContext = new BookEditorViewModel();
        }
    }
}
