using GUI.Client.ViewModel;
using System.Windows;

namespace GUI.Client
{
    /// <summary>
    /// Interaction logic for ConnectionSettingsView.xaml
    /// </summary>
    public partial class ConnectionSettingsView : Window
    {
        public ConnectionSettingsView(ConnectionSettings connectionSettings)
        {
            InitializeComponent();

            Loaded += (s, e) => DataContext = connectionSettings;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
