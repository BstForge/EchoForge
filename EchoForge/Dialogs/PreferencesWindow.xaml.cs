using System.Windows;

namespace EchoForge.Dialogs
{
    public partial class PreferencesWindow : Window
    {
        public PreferencesWindow()
        {
            InitializeComponent();
            TransitionsCheckBox.IsChecked = AppSettings.EnableTransitions;
        }

        private void TransitionsCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            AppSettings.EnableTransitions = TransitionsCheckBox.IsChecked == true;
        }
    }
}
