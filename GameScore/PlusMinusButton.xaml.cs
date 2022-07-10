using GameScore.Settings;
using System.Windows;
using System.Windows.Controls;

namespace GameScore
{
    /// <summary>
    /// Interaction logic for PlusMinusButton.xaml
    /// </summary>
    public partial class PlusMinusButton : UserControl
    {
        public PlusMinusButton()
        {
            InitializeComponent();
        }

        private Team Current => this.DataContext as Team;

        private void OnBonus(object sender, RoutedEventArgs e)
        {
            Current.Bonus = !Current.Bonus;
        }

        private void OnFoul(object sender, RoutedEventArgs e)
        {
            Current.AddFoul((sender as Button).Content.ToString() == "-" ? -1 : 1);
        }
    }
}
