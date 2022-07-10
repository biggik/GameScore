using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GameScore.Settings
{
    public class Team : INotifyPropertyChanged
    {
        private ImageSource image;
        private string name;
        private int score;
        private bool bonus;
        private string scoreText;

        public ImageSource Image
        {
            get => image;
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                InvokePropertyChanged(nameof(Name));

                UpdateImage();
            }
        }

        private void UpdateImage()
        {
            var cwd = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (cwd.GetDirectories("Resources").Length == 0)
            {
                cwd = cwd.Parent;
            }

            var images = new DirectoryInfo(Path.Combine(cwd.FullName, "Resources", "Icons")).GetFiles("*.*");
            var found = images.FirstOrDefault(x => x.Name.ToLower().StartsWith(name.ToLower())); 
            if (found != null)
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(found.FullName);
                bmp.EndInit();
                image = bmp;

                InvokePropertyChanged(nameof(Image));
            }
        }

        public int Score
        {
            get => score;
            set
            {
                score = value;
                InvokePropertyChanged(nameof(Score));
                ScoreText = score.ToString();
            }
        }

        public string ScoreText
        {
            get => scoreText;
            set
            { 
                scoreText = value; 
                InvokePropertyChanged(nameof(ScoreText));
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

            UpdateFouls();
        }

        internal void UpdateFouls()
        {
            var p = GameClockSettings.Instance.Period - 1;
            Bonus = Fouls[p] > 4;

            InvokePropertyChanged(nameof(FoulsInfo));
            InvokePropertyChanged(nameof(Bonus));
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
