using System.Windows;
using System.Windows.Controls;
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
        
        private void HomeTeamDown(object sender, RoutedEventArgs e)
        {
            GameClockSettings.Instance.UpdateHomeScore(-1);
        }

        private void HomeTeamUp(object sender, RoutedEventArgs e)
        {
            GameClockSettings.Instance.UpdateHomeScore(int.Parse((sender as Button).Content.ToString()));
        }

        private void HomeTeamBonus(object sender, RoutedEventArgs e)
        {
            GameClockSettings.Instance.UpdateHomeBonus(!GameClockSettings.Instance.HomeBonus);
        }

        private void GuestTeamDown(object sender, RoutedEventArgs e)
        {
            GameClockSettings.Instance.UpdateGuestScore(-1);
        }

        private void GuestTeamUp(object sender, RoutedEventArgs e)
        {
            GameClockSettings.Instance.UpdateGuestScore(int.Parse((sender as Button).Content.ToString()));
        }

        private void GamePeriodDown(object sender, RoutedEventArgs e)
        {
            GameClockSettings.Instance.UpdateGamePeriod(-1);
        }
        private void GamePeriodUp(object sender, RoutedEventArgs e)
        {
            GameClockSettings.Instance.UpdateGamePeriod(+1);
        }

        private void GuestTeamBonus(object sender, RoutedEventArgs e)
        {
            GameClockSettings.Instance.UpdateGuestBonus(!GameClockSettings.Instance.GuestBonus);
        }

        private void Settings(object sender, RoutedEventArgs e)
        {
            var wnd = new SettingsWindow();
            wnd.ShowDialog();
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            GameClockSettings.Instance.HomeScore = 0;
            GameClockSettings.Instance.UpdateHomeScore(0);
            GameClockSettings.Instance.GuestScore = 0;
            GameClockSettings.Instance.UpdateGuestScore(0);

            GameClockSettings.Instance.UpdateHomeBonus(false);
            GameClockSettings.Instance.UpdateGuestBonus(false);

            GameClockSettings.Instance.UpdateGamePeriod(-100);
        }
    }
}
