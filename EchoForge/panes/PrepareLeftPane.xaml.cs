using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EchoForge.Services;

namespace EchoForge.panes
{
    public partial class PrepareLeftPane : UserControl
    {
        public PrepareLeftPane()
        {
            InitializeComponent();

            InputTextBox.Text = MainWindow.PrepareText;
            BookField.Text = MainWindow.BookTitle;
            ChapterField.Text = MainWindow.ChapterNumber;
            SceneField.Text = MainWindow.SceneNumber;
            UpdateCharCount();
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MainWindow.PrepareText = InputTextBox.Text;
            UpdateCharCount();
        }

        private void UpdateCharCount()
        {
            var count = InputTextBox.Text.Length;
            CharCountText.Text = $"{count} characters";
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(InputTextBox.Text);
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                InputTextBox.Text = Clipboard.GetText();
            }
        }

        private async void Parse_Click(object sender, RoutedEventArgs e)
        {
            var text = InputTextBox.Text;
            var chunks = text.Split(new[] { "@@@@@" }, StringSplitOptions.RemoveEmptyEntries);
            if (chunks.Length == 0)
                chunks = new[] { text };

            if (chunks.Any(c => c.Length > 4800))
            {
                MessageBox.Show("Input is too long. Please choose Auto-Chunk or manually split the scene.", "Too Long");
                return;
            }

            var apiKey = Properties.Settings.Default.ChatGPTApiKey;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                MessageBox.Show("ChatGPT API key not set.", "Parse");
                return;
            }

            var service = new ChatGPTService(apiKey);
            var results = new List<string>();
            foreach (var c in chunks)
            {
                var res = await service.ParseSceneAsync(c, MainWindow.VoiceMode == "Narration & Dialog");
                results.Add(res);
            }

            MainWindow.BookTitle = BookField.Text;
            MainWindow.ChapterNumber = ChapterField.Text;
            MainWindow.SceneNumber = SceneField.Text;

            MessageBox.Show(string.Join("\n\n", results), "Parse Result");
        }
    }
}

