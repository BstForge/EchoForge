using Fluent;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace EchoForge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Theme toggled (placeholder).", "Toggle Theme");
        }

        private void EditApiKeys_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("API key editor placeholder.", "Edit API Keys");
        }

        private void ConfiguredVoices_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Configured voices placeholder.", "Configured Voices");
        }

        private void EnterProductKey_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Product key entry placeholder.", "Enter Product Key");
        }

        private void JoinDiscord_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://discord.gg/qs2sY8yv") { UseShellExecute = true });
        }

        private void Support_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Support placeholder.", "Support");
        }

        private void AboutApp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("EchoForge v1.0.0", "About EchoForge", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AboutDev_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Developed by Your Name\nContact: example@example.com", "About Developer", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}