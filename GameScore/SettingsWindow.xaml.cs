using System.Diagnostics;
using System.Windows;
using GameScore.Settings;

namespace GameScore
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            DataContext = GameClockSettings.Instance;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            GameClockSettings.Instance.Save();

            this.Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GoToFileLocation(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start("Explorer.exe", GameClockSettings.Instance.FileLocations);
        }
    }
}
