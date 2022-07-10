using System;
using System.ComponentModel;

namespace GameScore.Settings
{
    public class Team : INotifyPropertyChanged
    {
        private string name;
        private int score;
        private bool bonus;

        public string Name
        {
            get => name;
            set
            {
                name = value;
                InvokePropertyChanged(nameof(Name));
            }
        }
        public int Score
        {
            get => score;
            set
            {
                score = value;
                InvokePropertyChanged(nameof(Score));
            }
        }
        public bool Bonus
        {
            get => bonus;
            set 
            { 
                bonus = value; 
                InvokePropertyChanged(nameof(Bonus));
            }
        }
        public int[] Fouls { get; private set; } = new int[10];
        public string FoulsInfo
        {
            get
            {
                var p = GameClockSettings.Instance.Period - 1;
                return Fouls[p] > 4
                    ? $"{GameClockSettings.Instance.Texts.Bonus} [{Fouls[p]}]"
                    : $"{GameClockSettings.Instance.Texts.Fouls} [{Fouls[p]}]";
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        internal void AddFoul(int delta)
        {
            var p = GameClockSettings.Instance.Period - 1;
            Fouls[p] = Math.Max(0, Fouls[p] + delta);
            Bonus = Fouls[p] > 4;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FoulsInfo)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Bonus)));
        }

        private void InvokePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        internal void Reset()
        {
            Score = 0;
            Bonus = false;
            Fouls = new int[10];
            AddFoul(-100);
        }
    }
}
