using GameScore.Settings;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace GameScore
{
    /// <summary>
    /// Team name, score and bonus
    /// </summary>
    public partial class TeamInfo : UserControl
    {
        private readonly BlockingCollection<int> _deltas = new();
        private readonly DispatcherTimer dispatcherTimer;

        public TeamInfo()
        {
            InitializeComponent();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += OnTimerElapsed;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(2500);
        }

        private void OnTimerElapsed(object sender, EventArgs e)
        {
            while (_deltas.TryTake(out int delta))
            {
                UpdateScore(delta, false);
            }
            dispatcherTimer.Stop();
        }

        public string Id { get; set; }

        private Team Current => this.DataContext as Team;

        private void UpdateScore(int delta, bool withAnimation)
        {
            if (withAnimation)
            {
                Current.ScoreText = delta > 0 ? $"+{delta}" : $"-{delta}";
                File.WriteAllText(Path.Combine(GameClockSettings.Instance.FileLocations, $"{Id}{nameof(Team.Score)}.txt"), Current.ScoreText);

                _deltas.TryAdd(delta);
                if (!dispatcherTimer.IsEnabled)
                {
                    dispatcherTimer.Start();
                }
            }
            else
            {
                Current.Score = Math.Max(0, Current.Score + delta);
                File.WriteAllText(Path.Combine(GameClockSettings.Instance.FileLocations, $"{Id}{nameof(Team.Score)}.txt"), Current.Score.ToString());
            }
        }

        private void ScoreDown(object sender, RoutedEventArgs e)
        {
            UpdateScore(-1, false);
        }

        private void ScoreUp(object sender, RoutedEventArgs e)
        {
            UpdateScore(int.Parse((sender as Button).Content.ToString()), true);
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
