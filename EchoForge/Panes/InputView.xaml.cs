using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EchoForge.Panes
{
    /// <summary>
    /// Interaction logic for InputView.xaml
    /// </summary>
    public partial class InputView : UserControl
    {
        public InputView()
        {
            InitializeComponent();
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = InputTextBox.Text;
            int charCount = text.Length;

            int wordCount = 0;
            if (!string.IsNullOrWhiteSpace(text))
            {
                wordCount = text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Length;
            }

            WordCountText.Text = $"Words: {wordCount}";
            CharCountText.Text = $"Characters: {charCount}";
        }
    }
}
