using System.Windows;
using GameScore.Settings;

namespace GameScore
{
    public partial class GameScoreWindow : Window
    {
        public GameScoreWindow()
        {
            InitializeComponent();

            DataContext = GameClockSettings.Instance;
        }
        
        private void GamePeriodDown(object sender, RoutedEventArgs e)
        {
            GameClockSettings.Instance.UpdateGamePeriod(-1);
        }
        private void GamePeriodUp(object sender, RoutedEventArgs e)
        {
            GameClockSettings.Instance.UpdateGamePeriod(+1);
        }

        private void Settings(object sender, RoutedEventArgs e)
        {
            var wnd = new SettingsWindow();
            wnd.ShowDialog();
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            GameClockSettings.Instance.Home.Reset();
            GameClockSettings.Instance.Guests.Reset();
            GameClockSettings.Instance.UpdateGamePeriod(-100);
        }
    }
}
