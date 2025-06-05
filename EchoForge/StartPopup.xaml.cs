using System;
using System.Windows;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace EchoForge
{
    public partial class StartPopup : Window
    {
        private DispatcherTimer _timer;
        private int _seconds = 10;

        public StartPopup()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
            UpdateButtonContent();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _seconds--;
            UpdateButtonContent();
            if (_seconds <= 0)
            {
                _timer.Stop();
                CloseButton.IsEnabled = true;
            }
        }

        private void UpdateButtonContent()
        {
            CloseButton.Content = $"Close ({_seconds})";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}