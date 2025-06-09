using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using EchoForge.Models;

namespace EchoForge.panes
{
    public partial class PrepareRightPane : UserControl
    {
        private List<Chunk> _chunks = new();
        private int _index;

        public PrepareRightPane()
        {
            InitializeComponent();
        }

        public void LoadChunks(List<Chunk> chunks)
        {
            _chunks = chunks;
            _index = 0;
            DisplayCurrent();
        }

        private void PrevChunk_Click(object sender, RoutedEventArgs e)
        {
            if (_index > 0)
            {
                _index--;
                DisplayCurrent();
            }
        }

        private void NextChunk_Click(object sender, RoutedEventArgs e)
        {
            if (_index < _chunks.Count - 1)
            {
                _index++;
                DisplayCurrent();
            }
        }

        private void DisplayCurrent()
        {
            if (!_chunks.Any())
            {
                ChunkBox.Document.Blocks.Clear();
                ToneBox.Text = string.Empty;
                SpeedSlider.Value = 0;
                PacingSlider.Value = 0;
                VoiceDropdown.Items.Clear();
                return;
            }

            var c = _chunks[_index];
            ChunkBox.Document.Blocks.Clear();
            ChunkBox.Document.Blocks.Add(new Paragraph(new Run(c.Text)));
            ToneBox.Text = c.Tone;
            SpeedSlider.Value = ClampSlider(c.Speed);
            PacingSlider.Value = ClampSlider(c.Pacing);
            UpdateVoiceOptions(c.Voice);
        }

        private static int ClampSlider(int val)
        {
            val = val - 1;
            if (val < 0) return 0;
            if (val > 9) return 9;
            return val;
        }

        private void UpdateVoiceOptions(string chunkVoice)
        {
            VoiceDropdown.Items.Clear();
            IEnumerable<VoiceInfo> voices;
            if (chunkVoice.Equals("Narrator", System.StringComparison.OrdinalIgnoreCase))
            {
                voices = MainWindow.AllVoices.Where(v => v.IsNarrator && !v.DoNotUse);
            }
            else
            {
                voices = MainWindow.AllVoices.Where(v => v.IsDialog && !v.DoNotUse);
            }

            foreach (var v in voices)
            {
                VoiceDropdown.Items.Add(v.name);
            }

            if (VoiceDropdown.Items.Count > 0)
                VoiceDropdown.SelectedIndex = 0;
        }
    }
}