using EchoForge.Models;
using NAudio.Wave;
using System.Windows;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;

namespace EchoForge.panes
{
    public partial class SettingsLeftPane : UserControl
    {
        public SettingsLeftPane()
        {
            InitializeComponent();
            SetInitialMode();
        }

        private void SetInitialMode()
        {
            foreach (ComboBoxItem item in VoiceModeDropdown.Items)
            {
                if ((string)item.Content == MainWindow.VoiceMode)
                {
                    VoiceModeDropdown.SelectedItem = item;
                    break;
                }
            }
        }

        public void LoadVoices(List<VoiceInfo> voices)
        {
            VoicesPanel.Children.Clear();
            foreach (var v in voices)
            {
                var container = new StackPanel { Margin = new Thickness(0, 0, 0, 10) };

                var header = new StackPanel { Orientation = Orientation.Horizontal };
                header.Children.Add(new TextBlock { Text = v.name, FontWeight = FontWeights.Bold });
                if (v.labels != null)
                {
                    foreach (var lab in v.labels.Values)
                    {
                        header.Children.Add(new TextBlock
                        {
                            Text = lab,
                            Margin = new Thickness(5, 0, 0, 0),
                            Background = Brushes.LightGray,
                            Padding = new Thickness(3, 0, 3, 0)
                        });
                    }
                }
                container.Children.Add(header);

                var checkPanel = new StackPanel { Orientation = Orientation.Horizontal };
                var cbNarr = new CheckBox { Content = "Narrator", IsChecked = v.IsNarrator };
                var cbDialog = new CheckBox { Content = "Dialog", Margin = new Thickness(5, 0, 0, 0), IsChecked = v.IsDialog };
                var cbDoNot = new CheckBox { Content = "Do Not Use", Margin = new Thickness(5, 0, 0, 0), IsChecked = v.DoNotUse };

                void UpdateHeaderColor()
                {
                    if (v.DoNotUse)
                    {
                        header.Background = Brushes.Red;
                    }
                    else if (v.IsNarrator && v.IsDialog)
                    {
                        header.Background = Brushes.MediumPurple;
                    }
                    else if (v.IsNarrator)
                    {
                        header.Background = Brushes.LightBlue;
                    }
                    else if (v.IsDialog)
                    {
                        header.Background = Brushes.MediumSeaGreen;
                    }
                    else
                    {
                        header.Background = Brushes.Transparent;
                    }
                }

                cbNarr.Checked += (s, e) => { v.IsNarrator = true; cbDoNot.IsChecked = false; UpdateHeaderColor(); };
                cbNarr.Unchecked += (s, e) => { v.IsNarrator = false; UpdateHeaderColor(); };
                cbDialog.Checked += (s, e) => { v.IsDialog = true; cbDoNot.IsChecked = false; UpdateHeaderColor(); };
                cbDialog.Unchecked += (s, e) => { v.IsDialog = false; UpdateHeaderColor(); };
                cbDoNot.Checked += (s, e) => { v.DoNotUse = true; cbNarr.IsChecked = false; cbDialog.IsChecked = false; UpdateHeaderColor(); };
                cbDoNot.Unchecked += (s, e) => { v.DoNotUse = false; UpdateHeaderColor(); };

                checkPanel.Children.Add(cbNarr);
                checkPanel.Children.Add(cbDialog);
                checkPanel.Children.Add(cbDoNot);

                Button? playButton = null;
                WaveOutEvent? player = null;
                Mp3FileReader? reader = null;
                MemoryStream? ms = null;
                bool playing = false;

                if (!string.IsNullOrEmpty(v.preview_url))
                {
                    playButton = new Button { Content = "Play", Margin = new Thickness(10, 0, 0, 0), Width = 60 };
                    playButton.Click += async (s, e) =>
                    {
                        if (!playing)
                        {
                            if (player == null)
                            {
                                try
                                {
                                    using var client = new HttpClient();
                                    var data = await client.GetByteArrayAsync(v.preview_url!);
                                    ms = new MemoryStream(data);
                                    reader = new Mp3FileReader(ms);
                                    player = new WaveOutEvent();
                                    player.Init(reader);
                                    player.PlaybackStopped += (s2, e2) =>
                                    {
                                        playButton.Content = "Play";
                                        playing = false;
                                    };
                                }
                                catch
                                {
                                    return;
                                }
                            }

                            player.Play();
                            playButton.Content = "Stop";
                            playing = true;
                        }
                        else
                        {
                            player?.Stop();
                            playButton.Content = "Play";
                            playing = false;
                        }
                    };
                }

                var bottomPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 2, 0, 2) };
                bottomPanel.Children.Add(checkPanel);
                if (playButton != null)
                    bottomPanel.Children.Add(playButton);

                container.Children.Add(bottomPanel);

                VoicesPanel.Children.Add(container);
                UpdateHeaderColor();
            }
        }

        private async Task PlayPreview(string url)
        {
            try
            {
                using var client = new HttpClient();
                var data = await client.GetByteArrayAsync(url);
                using var ms = new MemoryStream(data);
                using var reader = new Mp3FileReader(ms);
                using var waveOut = new WaveOutEvent();
                waveOut.Init(reader);
                waveOut.Play();
            }
            catch { }
        }

        private void VoiceModeDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VoiceModeDropdown.SelectedItem is ComboBoxItem item && item.Content is string mode)
            {
                MainWindow.VoiceMode = mode;
            }
        }
    }
}