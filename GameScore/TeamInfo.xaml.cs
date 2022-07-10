using GameScore.Settings;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace GameScore
{
    /// <summary>
    /// Team name, score and bonus
    /// </summary>
    public partial class TeamInfo : UserControl
    {
        public TeamInfo()
        {
            InitializeComponent();
        }

        public string Id { get; set; }

        private Team Current => this.DataContext as Team;

        private void UpdateScore(int delta)
        {
            Current.Score = Math.Max(0, Current.Score + delta);
            File.WriteAllText(Path.Combine(GameClockSettings.Instance.FileLocations, $"{Id}{nameof(Team.Score)}.txt"), Current.Score.ToString());
        }

        private void ScoreDown(object sender, RoutedEventArgs e)
        {
            UpdateScore(-1);
        }

        private void ScoreUp(object sender, RoutedEventArgs e)
        {
            UpdateScore(int.Parse((sender as Button).Content.ToString()));
        }

        public void UpdateBonus(bool isBonus)
        {
            Current.Bonus = isBonus;
            File.WriteAllText(Path.Combine(GameClockSettings.Instance.FileLocations, $"{Id}{nameof(Team.Bonus)}.txt"), 
                Current.Bonus ? GameClockSettings.Instance.Texts.Bonus : "");
        }

        private void TeamFoul(object sender, RoutedEventArgs e)
        {
            var delta = (sender as Button).Content.ToString() == "-" ? -1 : 1;
            Current.AddFoul(delta);
        }
    }
}
