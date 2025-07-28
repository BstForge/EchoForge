using System.Windows;
using System.Windows.Controls;
using EchoForge.Panes;

namespace EchoForge
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Load the default view
            LoadView(new InputView());
        }

        private void LoadView(UserControl view)
        {
            MainContentControl.Content = view;
        }

        // Button click handlers
        private void Input_Click(object sender, RoutedEventArgs e) => LoadView(new InputView());
        private void Parsed_Click(object sender, RoutedEventArgs e) => LoadView(new ParsedView());
        private void Generate_Click(object sender, RoutedEventArgs e) => LoadView(new GenerateView());
        private void Audio_Click(object sender, RoutedEventArgs e) => LoadView(new AudioFilesView());
        private void Voice_Click(object sender, RoutedEventArgs e) => LoadView(new VoiceOptionsView());
    }
}
