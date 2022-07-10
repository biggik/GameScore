using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;

namespace GameScore.Settings
{
    public class GameClockSettings : INotifyPropertyChanged
    {
        private static readonly Lazy<GameClockSettings> _instance = new(() => new GameClockSettings());

        public event PropertyChangedEventHandler PropertyChanged;

        private GameClockSettings()
        {
            FileLocations = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GameClock");
            Directory.CreateDirectory(FileLocations);

            GameName = FromFile(nameof(GameName), "Can be modified in Settings");
            GameDescription = FromFile(nameof(GameDescription));

            Home.Name = FromFile("HomeTeam", "Home");
            Home.Score = AsInt(FromFile("HomeScore"));
            Home.Bonus = AsBool(FromFile("HomeBonus"));

            Guests.Name = FromFile("GuestTeam", "Guest");
            Guests.Score = AsInt(FromFile("GuestScore"));
            Guests.Bonus = AsBool(FromFile("GuestBonus"));

            Period = int.TryParse(FromFile(nameof(GamePeriod)), out int p) ? p : 1;

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
            static string WithDefault(string current, string defaultValue)
            {
                return string.IsNullOrWhiteSpace(current) ? defaultValue : current;
            }

            var ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (ci.Name == "is-IS")
            {
                Texts.Bonus = WithDefault(Texts.Bonus, "Bónus");
                Texts.Fouls = WithDefault(Texts.Fouls, "Villur");
                Texts.Overtime = WithDefault(Texts.Overtime, "Fl {0}");
                Texts.PeriodText = WithDefault(Texts.PeriodText, "Lh {0}");
            }
            else
            {
                Texts.Bonus = WithDefault(Texts.Bonus, "Bonus");
                Texts.Fouls = WithDefault(Texts.Fouls, "Fouls");
                Texts.Overtime = WithDefault(Texts.Overtime, "OT {0}");
                Texts.PeriodText = WithDefault(Texts.PeriodText, "P{0}");
            }
        }

        internal void UpdateGamePeriod(int delta)
        {
            Period += delta;
            if (Period < 1)
            {
                Period = 1;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Period)));
            File.WriteAllText(Path.Combine(FileLocations, nameof(Period) + ".txt"), Period.ToString());

            GamePeriod = Period <= 4 ? Period.ToString() : string.Format(Texts.Overtime, Period - 4);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GamePeriod)));
            File.WriteAllText(Path.Combine(FileLocations, nameof(GamePeriod) + ".txt"), GamePeriod);

            try
            {
                GamePeriodText = Period < 5 ? string.Format(Texts.PeriodText, GamePeriod) : string.Format(Texts.Overtime, Period - 4);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GamePeriodText)));
                File.WriteAllText(Path.Combine(FileLocations, nameof(GamePeriodText) + ".txt"), GamePeriodText);
            }
            catch { }
        }

        public LocalizedTexts Texts { get; set; }

        private static int AsInt(string value) => int.TryParse(value, out int i) ? i : 0;
        private static bool AsBool(string value) => bool.TryParse(value, out bool i) && i;

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

        public Team Home {get; set; } = new Team();

        public Team Guests { get; set; } = new Team();

        public int Period { get; set; }
        public string GamePeriod { get; set; }
        public string GamePeriodText { get; set; }

        public static GameClockSettings Instance => _instance.Value;

        public void Save()
        {
            Directory.CreateDirectory(FileLocations);

            File.WriteAllText(Path.Combine(FileLocations, nameof(GameName) + ".txt"), GameName);
            File.WriteAllText(Path.Combine(FileLocations, nameof(GameDescription) + ".txt"), GameDescription);
            File.WriteAllText(Path.Combine(FileLocations, "HomeTeam.txt"), Home.Name);
            File.WriteAllText(Path.Combine(FileLocations, "GuestTeam.txt"), Guests.Name);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GameName)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GameDescription)));
        }
    }
}
