using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
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

        private Storyboard GetStoryboard(string key, DependencyObject target)
        {
            var sb = (Storyboard)FindResource(key);
            sb = sb.Clone();
            foreach (var timeline in sb.Children)
            {
                Storyboard.SetTarget(timeline, target);
            }
            return sb;
        }

        private Task BeginStoryboardAsync(Storyboard storyboard)
        {
            var tcs = new TaskCompletionSource<bool>();
            if (storyboard == null)
            {
                tcs.SetResult(true);
                return tcs.Task;
            }
            void End(object? s, EventArgs e)
            {
                storyboard.Completed -= End;
                tcs.SetResult(true);
            }
            storyboard.Completed += End;
            storyboard.Begin();
            return tcs.Task;
        }

        private async Task TransitionAsync(UserControl view, UserControl ribbon)
        {
            var contentRetract = GetStoryboard("ContentRetractStoryboard", MainContentBorder);
            var ribbonRetract = GetStoryboard("RibbonRetractStoryboard", RibbonContainer);
            var ribbonExpand = GetStoryboard("RibbonExpandStoryboard", RibbonContainer);
            var contentExpand = GetStoryboard("ContentExpandStoryboard", MainContentBorder);

            await BeginStoryboardAsync(contentRetract);
            await BeginStoryboardAsync(ribbonRetract);

            LoadView(view);
            SetRibbon(ribbon);

            await BeginStoryboardAsync(ribbonExpand);
            await BeginStoryboardAsync(contentExpand);
        }

        // Button click handlers
        private async void Input_Click(object sender, RoutedEventArgs e)
        {
            await TransitionAsync(new InputView(), _inputRibbon);
        }

        private async void Parsed_Click(object sender, RoutedEventArgs e)
        {
            await TransitionAsync(new ParsedView(), _parsedRibbon);
        }

        private async void Generate_Click(object sender, RoutedEventArgs e)
        {
            await TransitionAsync(new GenerateView(), _generateRibbon);
        }

        private async void Audio_Click(object sender, RoutedEventArgs e)
        {
            await TransitionAsync(new AudioFilesView(), _audioRibbon);
        }

        private async void Voice_Click(object sender, RoutedEventArgs e)
        {
            await TransitionAsync(new VoiceOptionsView(), _voiceRibbon);
        }
    }
}
