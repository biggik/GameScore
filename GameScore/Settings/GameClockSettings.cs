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

            GamePeriod = FromFile(nameof(GamePeriod));
            if (string.IsNullOrWhiteSpace(GamePeriod))
            {
                GamePeriod = "1";
            }
            GamePeriodText = FromFile(nameof(GamePeriodText));

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

                Localize();
            }
            else
            {
                Texts = new LocalizedTexts();
                Localize();
            }
         
            File.WriteAllText(textsFile, JsonSerializer.Serialize(Texts, new JsonSerializerOptions { WriteIndented = true }));
        }

        private void Localize()
        {
            string WithDefault(string current, string defaultValue)
            {
                return string.IsNullOrWhiteSpace(current) ? defaultValue : current;
            }

            var ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (ci.Name == "is-IS")
            {
                Texts.Bonus = WithDefault(Texts.Bonus, "Bónus");
                Texts.Overtime = WithDefault(Texts.Overtime, "Frl.");
                Texts.PeriodText = WithDefault(Texts.PeriodText, "Lh {0}");
            }
            else
            {
                Texts.Bonus = WithDefault(Texts.Bonus, "Bonus");
                Texts.Overtime = WithDefault(Texts.Overtime, "OT");
                Texts.PeriodText = WithDefault(Texts.PeriodText, "P{0}");
            }
        }
        internal void UpdateGamePeriod(int delta)
        {
            if (!int.TryParse(GamePeriod, out int current))
            {
                if (GamePeriod == Texts.Overtime)
                {
                    current = 5;
                }
                else
                {
                    current = 1;
                }
                current += delta;
            }
            else
            {
                current += delta;
            }

            current = Math.Min(5, Math.Max(current, 1));

            GamePeriod = current == 1 ? "1"
                       : current == 2 ? "2"
                       : current == 3 ? "3"
                       : current == 4 ? "4"
                       : Texts.Overtime;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GamePeriod)));
            File.WriteAllText(Path.Combine(FileLocations, nameof(GamePeriod) + ".txt"), GamePeriod);

            try
            {
                GamePeriodText = current < 5 ? string.Format(Texts.PeriodText, GamePeriod) : GamePeriod;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GamePeriodText)));
                File.WriteAllText(Path.Combine(FileLocations, nameof(GamePeriodText) + ".txt"), GamePeriodText);
            }
            catch { }
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

        public string GamePeriod { get; set; }
        public string GamePeriodText { get; set; }

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
