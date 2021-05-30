using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;

namespace GameScore.Settings
{
    public class GameClockSettings : INotifyPropertyChanged
    {
        private static Lazy<GameClockSettings> _instance = new Lazy<GameClockSettings>(() => new GameClockSettings());

        public event PropertyChangedEventHandler PropertyChanged;

        private GameClockSettings()
        {
            FileLocations = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GameClock");
            Directory.CreateDirectory(FileLocations);

            GameName = FromFile(nameof(GameName), "Can be modified in Settings");
            GameDescription = FromFile(nameof(GameDescription));

            HomeTeam = FromFile(nameof(HomeTeam), "Home");
            HomeScore = AsInt(FromFile(nameof(HomeScore)));
            HomeBonus = AsBool(FromFile(nameof(HomeBonus)));

            GuestTeam = FromFile(nameof(GuestTeam), "Guest");
            GuestScore = AsInt(FromFile(nameof(GuestScore)));
            GuestBonus = AsBool(FromFile(nameof(GuestBonus)));

            var textsFile = Path.Combine(FileLocations, "texts.json");
            if (File.Exists(textsFile))
            {
                try
                {
                    Texts = JsonSerializer.Deserialize<LocalizedTexts>(File.ReadAllText(textsFile));
                }
                catch
                {
                    Texts = new LocalizedTexts();
                }
            }
            else
            {
                Texts = new LocalizedTexts();
                var ci = System.Threading.Thread.CurrentThread.CurrentCulture;
                if (ci.Name == "is-IS")
                {
                    Texts.Bonus = "Bónus";
                }
                File.WriteAllText(textsFile, JsonSerializer.Serialize<LocalizedTexts>(Texts, new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        public LocalizedTexts Texts { get; set; }

        private int AsInt(string value) => int.TryParse(value, out int i) ? i : 0;
        private bool AsBool(string value) => bool.TryParse(value, out bool i) ? i : false;

        private string FromFile(string filePart, string defaultValue = "")
        {
            var fullFile = Path.Combine(FileLocations, filePart + ".txt");
            if (File.Exists(fullFile))
            {
                return File.ReadAllText(fullFile);
            }
            return defaultValue;
        }


        public string FileLocations { get; set; }
        public string GameName { get; set; }
        public string GameDescription { get; set; }
        
        public string HomeTeam { get; set; }
        public int HomeScore { get; set; }
        public bool HomeBonus { get; set; }
        
        public string GuestTeam { get; set; }
        public int GuestScore { get; set; }
        public bool GuestBonus { get; set; }

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
        public void UpdateHomeBonus(bool isBonus)
        {
            HomeBonus = isBonus;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HomeBonus)));
            File.WriteAllText(Path.Combine(FileLocations, nameof(HomeBonus) + ".txt"), HomeBonus ? Texts.Bonus : "");
        }
        public void UpdateGuestBonus(bool isBonus)
        {
            GuestBonus = isBonus;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GuestBonus)));
            File.WriteAllText(Path.Combine(FileLocations, nameof(GuestBonus) + ".txt"), GuestBonus ? Texts.Bonus : "");
        }
    }
}
