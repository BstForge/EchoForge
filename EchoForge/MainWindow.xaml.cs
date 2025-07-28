using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using System.IO;
using EchoForge.Panes;
using EchoForge.Menus;
using EchoForge.Dialogs;
using EchoForge.Services;

namespace EchoForge
{
    public partial class MainWindow : Window
    {
        private readonly UserControl _inputRibbon = new InputRibbon();
        private readonly UserControl _parsedRibbon = new ParsedRibbon();
        private readonly UserControl _generateRibbon = new GenerateRibbon();
        private readonly UserControl _audioRibbon = new AudioRibbon();
        private readonly UserControl _voiceRibbon = new VoiceRibbon();
        private string _currentView = "Input";
        private string? _currentFilePath;

        public MainWindow()
        {
            InitializeComponent();

            Title = "EchoForge - New Project";

            // Load the default view and ribbon
            LoadView(new InputView());
            SetRibbon(_inputRibbon);
            _currentView = "Input";
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
            if (AppSettings.EnableTransitions)
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
            else
            {
                LoadView(view);
                SetRibbon(ribbon);
            }
        }

        private async Task SetViewAsync(string view)
        {
            switch (view)
            {
                case "Input":
                    await TransitionAsync(new InputView(), _inputRibbon);
                    break;
                case "Parsed":
                    await TransitionAsync(new ParsedView(), _parsedRibbon);
                    break;
                case "Generate":
                    await TransitionAsync(new GenerateView(), _generateRibbon);
                    break;
                case "Audio":
                    await TransitionAsync(new AudioFilesView(), _audioRibbon);
                    break;
                case "Voice":
                    await TransitionAsync(new VoiceOptionsView(), _voiceRibbon);
                    break;
                default:
                    await TransitionAsync(new InputView(), _inputRibbon);
                    view = "Input";
                    break;
            }
            _currentView = view;
        }

        // Button click handlers
        private async void Input_Click(object sender, RoutedEventArgs e)
        {
            await SetViewAsync("Input");
        }

        private async void Parsed_Click(object sender, RoutedEventArgs e)
        {
            await SetViewAsync("Parsed");
        }

        private async void Generate_Click(object sender, RoutedEventArgs e)
        {
            await SetViewAsync("Generate");
        }

        private async void Audio_Click(object sender, RoutedEventArgs e)
        {
            await SetViewAsync("Audio");
        }

        private async void Voice_Click(object sender, RoutedEventArgs e)
        {
            await SetViewAsync("Voice");
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            if (HamburgerButton.ContextMenu != null)
            {
                HamburgerButton.ContextMenu.PlacementTarget = HamburgerButton;
                HamburgerButton.ContextMenu.IsOpen = true;
            }
        }

        private void Preferences_Click(object sender, RoutedEventArgs e)
        {
            var pref = new PreferencesWindow { Owner = this };
            pref.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void New_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.EnableTransitions = true;
            AppSettings.Save();
            _currentFilePath = null;
            Title = "EchoForge - New Project";
            await SetViewAsync("Input");
        }

        private async void Load_Click(object sender, RoutedEventArgs e)
        {
            var result = ProjectService.Load();
            if (result.Data != null && result.Path != null)
            {
                AppSettings.EnableTransitions = result.Data.EnableTransitions;
                AppSettings.Save();
                await SetViewAsync(result.Data.SelectedView);
                _currentFilePath = result.Path;
                Title = $"EchoForge - {Path.GetFileNameWithoutExtension(result.Path)}";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var data = new ProjectData
            {
                SelectedView = _currentView,
                EnableTransitions = AppSettings.EnableTransitions,
                Manuscript = null
            };
            var path = ProjectService.Save(data);
            if (path != null)
            {
                _currentFilePath = path;
                Title = $"EchoForge - {Path.GetFileNameWithoutExtension(path)}";
            }
        }
    }
}
