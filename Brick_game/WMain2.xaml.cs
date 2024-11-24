using System.Windows;
using System.Windows.Controls;

namespace Brick_game {
    /// <summary>
    /// Interaction logic for WMain2.xaml
    /// </summary>
    public partial class WMain2 : Window {
        private Tetris _tetris;
        public TetrisViewModel tetrisViewModel;

        private int _highScoresShow = 5;

        public WMain2() {
            InitializeComponent();

            _tetris = new();
            tetrisViewModel = new(_tetris);
            this.DataContext = tetrisViewModel;

            //var topScores = _highScores.GetXFirstHighScores(_highScoresShow);
            spHighScoreLabels.ItemsSource = tetrisViewModel.TopScores;

            var dict = new ResourceDictionary();
            dict.Source = new Uri("Resources/StringResources.cs-CZ.xaml", UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        private void ChangeLanguage(string cultureCode) {
            var dict = new ResourceDictionary();
            dict.Source = new Uri($"Resources/StringResources.{cultureCode}.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
            if (tetrisViewModel != null) tetrisViewModel.UpdatebNewGame();
        }

        private void cbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var selectedItem = cbLanguage.SelectedItem as ComboBoxItem;
            if (selectedItem != null) {
                string cultureCode = selectedItem.Tag.ToString();
                ChangeLanguage(cultureCode);
            }
        }

        private void meMusic_MediaEnded(object sender, RoutedEventArgs e) {
            meMusic.Position = TimeSpan.Zero;
            meMusic.Play();
        }

        private void MIMusic_Unchecked(object sender, RoutedEventArgs e) {
            meMusic.Stop();
        }

        private void MIMusic_Checked(object sender, RoutedEventArgs e) {
            if (meMusic != null) meMusic.Play();
        }

        private void main2_Loaded(object sender, RoutedEventArgs e) {
            if (_tetris.Settings.PlayOnStartApp && MIMusic.IsChecked) { meMusic.Play(); }
            else { MIMusic.IsChecked = false; }
        }

        private void MIResetScores_Click(object sender, RoutedEventArgs e) {
            string mbResetScores = (string)Application.Current.Resources["MBResetScores"];
            string mbResetScoresCaption = (string)Application.Current.Resources["NamMBResetScoresCaptione"];
            MessageBoxResult result = MessageBox.Show(mbResetScores, mbResetScoresCaption, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK) {
                tetrisViewModel.HighScores.ClearAll();
                tetrisViewModel.HighScores.SaveToFile("highscores.xml");
                spHighScoreLabels.ItemsSource = tetrisViewModel.TopScores;
            }
        }

        private void MIHelp_Click(object sender, RoutedEventArgs e) {
            string MIHelpMessage = (string)Application.Current.Resources["MIHelpMessage"];
            MessageBox.Show(this, MIHelpMessage);
        }

        private void main2_GotFocus(object sender, RoutedEventArgs e) {
            spHighScoreLabels.ItemsSource = tetrisViewModel.TopScores;
        }
    }
}
