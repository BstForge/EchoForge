using System;
using System.Windows;
using EchoForge.Properties;

namespace EchoForge
{
    public partial class ApiKeyWindow : Window
    {

        public ApiKeyWindow()
        {
            InitializeComponent();
            LoadKeys();
        }

        private void LoadKeys()
        {
            if (!string.IsNullOrEmpty(Settings.Default.ChatGPTApiKey))
            {
                ChatGPTKeyTextBox.Text = MaskKey(Settings.Default.ChatGPTApiKey);
            }
            else
            {
                ChatGPTKeyTextBox.Text = string.Empty;
                ChatGPTKeyTextBox.PlaceholderText();
            }

            if (!string.IsNullOrEmpty(Settings.Default.ElevenLabsApiKey))
            {
                ElevenLabsKeyTextBox.Text = MaskKey(Settings.Default.ElevenLabsApiKey);
            }
            else
            {
                ElevenLabsKeyTextBox.Text = string.Empty;
                ElevenLabsKeyTextBox.PlaceholderText();
            }
        }

        private string MaskKey(string key)
        {
            if (key.Length <= 6) return key;
            return key.Substring(0, 2) + new string('*', key.Length - 6) + key[^4..];
        }

        private void SaveChatGPT_Click(object sender, RoutedEventArgs e)
        {
            var entered = ChatGPTKeyTextBox.Text.Trim();
            if (entered == MaskKey(Settings.Default.ChatGPTApiKey))
                return;
            if (string.IsNullOrWhiteSpace(entered))
                return;
            Settings.Default.ChatGPTApiKey = entered;
            Settings.Default.Save();
            ChatGPTKeyTextBox.Text = MaskKey(entered);
        }

        private void SaveElevenLabs_Click(object sender, RoutedEventArgs e)
        {
            var entered = ElevenLabsKeyTextBox.Text.Trim();
            if (entered == MaskKey(Settings.Default.ElevenLabsApiKey))
                return;
            if (string.IsNullOrWhiteSpace(entered))
                return;
            Settings.Default.ElevenLabsApiKey = entered;
            Settings.Default.Save();
            ElevenLabsKeyTextBox.Text = MaskKey(entered);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public static class TextBoxExtensions
    {
        public static void PlaceholderText(this System.Windows.Controls.TextBox box)
        {
            box.Text = box.Tag as string ?? string.Empty;
            box.Foreground = System.Windows.Media.Brushes.Gray;
            box.GotFocus += RemovePlaceholder;
            box.LostFocus += AddPlaceholder;
        }

        private static void RemovePlaceholder(object sender, RoutedEventArgs e)
        {
            var tb = sender as System.Windows.Controls.TextBox;
            if (tb != null && tb.Foreground == System.Windows.Media.Brushes.Gray)
            {
                tb.Text = string.Empty;
                tb.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private static void AddPlaceholder(object sender, RoutedEventArgs e)
        {
            var tb = sender as System.Windows.Controls.TextBox;
            if (tb != null && string.IsNullOrEmpty(tb.Text))
            {
                tb.Text = tb.Tag as string ?? string.Empty;
                tb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}