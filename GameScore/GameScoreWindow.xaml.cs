using System;
using System.ComponentModel;
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

        private void GuestTeamDown(object sender, RoutedEventArgs e)
        {
            GameClockSettings.Instance.UpdateGuestScore(-1);
        }

        private void GuestTeamUp(object sender, RoutedEventArgs e)
        {
            GameClockSettings.Instance.UpdateGuestScore(int.Parse((sender as Button).Content.ToString()));
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
        }
    }
}
