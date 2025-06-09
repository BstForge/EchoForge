using Fluent;
using System.Diagnostics;
using EchoForge.panes;
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
namespace EchoForge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public static List<Models.VoiceInfo> AllVoices { get; private set; } = new();
        public static string VoiceMode { get; set; } = "Single Narration";
        public static List<Models.Chunk> ParsedChunks { get; set; } = new();

        // Stores text and metadata when switching between panes
        public static string PrepareText { get; set; } = string.Empty;
        public static string BookTitle { get; set; } = string.Empty;
        public static string ChapterNumber { get; set; } = string.Empty;
        public static string SceneNumber { get; set; } = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            LeftPaneHost.Content = new panes.PrepareLeftPane();
            RightPaneHost.Content = new panes.PrepareRightPane();

            Loaded += (s, e) =>
            {
                var popup = new StartPopup();
                popup.Owner = this;
                popup.ShowDialog();
            };
        }

        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Theme toggled (placeholder).", "Toggle Theme");
        }

        private void EditApiKeys_Click(object sender, RoutedEventArgs e)
        {
            var win = new ApiKeyWindow();
            win.Owner = this;
            win.ShowDialog();
        }

        private async void CallVoices_Click(object sender, RoutedEventArgs e)
        {
            var key = Properties.Settings.Default.ElevenLabsApiKey;
            if (string.IsNullOrWhiteSpace(key))
            {
                MessageBox.Show("ElevenLabs API key not set.", "Call Voices");
                return;
            }

            try
            {
                var svc = new Services.ElevenLabsService();
                AllVoices = await svc.GetVoicesAsync(key);
                foreach (var v in AllVoices)
                {
                    v.DoNotUse = true;
                    v.IsNarrator = false;
                    v.IsDialog = false;
                }
                MessageBox.Show($"Retrieved {AllVoices.Count} voices.", "Call Voices");

                if (LeftPaneHost.Content is panes.SettingsLeftPane pane)
                {
                    pane.LoadVoices(AllVoices);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to fetch voices: {ex.Message}", "Call Voices");
            }
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

        private void MainRibbon_SelectedTabChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainRibbon.SelectedTabItem is RibbonTabItem selectedTab)
            {
                var header = selectedTab.Header as string;
                switch (header)
                {
                    case "Prepare":
                        LeftPaneHost.Content = new PrepareLeftPane();
                        RightPaneHost.Content = new PrepareRightPane();
                        break;
                    case "Generate":
                        LeftPaneHost.Content = new GenerateLeftPane();
                        RightPaneHost.Content = new GenerateRightPane();
                        break;
                    case "Settings":
                        var lp = new SettingsLeftPane();
                        LeftPaneHost.Content = lp;
                        RightPaneHost.Content = new SettingsRightPane();
                        if (AllVoices.Any())
                        {
                            lp.LoadVoices(AllVoices);
                        }
                        break;
                }
            }
        }
    }
}