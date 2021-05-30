using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameScore.Settings
{
    public class GameClockSettings : INotifyPropertyChanged
    {
        private static Lazy<GameClockSettings> _instance = new Lazy<GameClockSettings>(() => new GameClockSettings());

        public event PropertyChangedEventHandler PropertyChanged;

        private GameClockSettings()
        {
            FileLocations = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GameClock");

            GameName = FromFile(nameof(GameName), "Can be modified in Settings");
            GameDescription = FromFile(nameof(GameDescription));
            HomeTeam = FromFile(nameof(HomeTeam), "Home");
            GuestTeam = FromFile(nameof(GuestTeam), "Guest");
            HomeScore = AsInt(FromFile(nameof(HomeScore)));
            GuestScore = AsInt(FromFile(nameof(GuestScore)));
        }

        private int AsInt(string value)
        {
            return int.TryParse(value, out int i) ? i : 0;
        }

        private string FromFile(string filePart, string defaultValue = "")
        {
            var fullFile = Path.Combine(FileLocations, filePart + ".txt");
            if (File.Exists(fullFile))
            {
                return File.ReadAllText(fullFile);
            }
            return defaultValue;
        }


        public string GameName { get; set; }
        public string GameDescription { get; set; }
        public string HomeTeam { get; set; }
        public string GuestTeam { get; set; }
        public string FileLocations { get; set; }
        public int HomeScore { get; set; }
        public int GuestScore { get; set; }

        public static GameClockSettings Instance => _instance.Value;

        public void Save()
        {
            Directory.CreateDirectory(FileLocations);

            File.WriteAllText(Path.Combine(FileLocations, nameof(GameName) + ".txt"), GameName);
            File.WriteAllText(Path.Combine(FileLocations, nameof(GameDescription) + ".txt"), GameDescription);
            File.WriteAllText(Path.Combine(FileLocations, nameof(HomeTeam) + ".txt"), HomeTeam);
            File.WriteAllText(Path.Combine(FileLocations, nameof(GuestTeam) + ".txt"), GuestTeam);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GameName)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GameDescription)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HomeTeam)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GuestTeam)));

        }

        public void UpdateHomeScore(int delta)
        {
            HomeScore += delta;
            if (HomeScore < 0) HomeScore = 0;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HomeScore)));
            File.WriteAllText(Path.Combine(FileLocations, nameof(HomeScore) + ".txt"), HomeScore.ToString());
        }
        public void UpdateGuestScore(int delta)
        {
            GuestScore += delta;
            if (GuestScore < 0) GuestScore = 0;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GuestScore)));
            File.WriteAllText(Path.Combine(FileLocations, nameof(GuestScore) + ".txt"), GuestScore.ToString());
        }
    }
}
