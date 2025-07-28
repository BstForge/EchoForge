using System.Windows;
using System.Windows.Controls;
using EchoForge.Panes;
using EchoForge.Menus;

namespace EchoForge
{
    public partial class MainWindow : Window
    {
        private readonly UserControl _inputRibbon = new InputRibbon();
        private readonly UserControl _parsedRibbon = new ParsedRibbon();
        private readonly UserControl _generateRibbon = new GenerateRibbon();
        private readonly UserControl _audioRibbon = new AudioRibbon();
        private readonly UserControl _voiceRibbon = new VoiceRibbon();

        public MainWindow()
        {
            InitializeComponent();

            // Load the default view and ribbon
            LoadView(new InputView());
            SetRibbon(_inputRibbon);
        }

        private void LoadView(UserControl view)
        {
            MainContentControl.Content = view;
        }

        private void SetRibbon(UserControl ribbon)
        {
            MainRibbon.Content = ribbon;
        }

        // Button click handlers
        private void Input_Click(object sender, RoutedEventArgs e)
        {
            LoadView(new InputView());
            SetRibbon(_inputRibbon);
        }

        private void Parsed_Click(object sender, RoutedEventArgs e)
        {
            LoadView(new ParsedView());
            SetRibbon(_parsedRibbon);
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            LoadView(new GenerateView());
            SetRibbon(_generateRibbon);
        }

        private void Audio_Click(object sender, RoutedEventArgs e)
        {
            LoadView(new AudioFilesView());
            SetRibbon(_audioRibbon);
        }

        private void Voice_Click(object sender, RoutedEventArgs e)
        {
            LoadView(new VoiceOptionsView());
            SetRibbon(_voiceRibbon);
        }
    }
}
