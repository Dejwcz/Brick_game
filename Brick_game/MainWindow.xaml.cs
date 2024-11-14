using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Globalization;
using TXTS = Brick_game.Properties.Resources;
using System.IO;
using Brick_game;

namespace BrickGame; 
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {               
    private Grid gameField;

    private DispatcherTimer timer;
    private int countOfHighscores = 5;
    private HighScores highScores;
    private string highScoresFile = "highscores.xml";
    //private AppSettings appSettings;
    private string settingsFile = "settings.xml";

    private Tetris _tetris;
    private TetrisViewModel _tetrisViewModel;

    public MainWindow() {

        //!!!Do not forget to update LanguageChange() and WTetris_Loaded() if there was added new UI element with text. Data binding does not working well in this window 

        InitializeComponent();

        _tetris = new Tetris(MainContainer, timer);
        _tetrisViewModel = new TetrisViewModel(_tetris);
        this.DataContext = _tetrisViewModel;


        highScores = HighScores.LoadFromFile(highScoresFile);                                       //Load high scores from file
        SetHighScores(highScores.GetXFirstHighScoresToString(countOfHighscores));                   //Set high scores to Labels

        _tetris.SetUI();
        //WTetris.Height = gameField.Height + 120 < 480 ? 480 : gameField.Height + 120;
        //WTetris.Width = gameField.Width + 310 < 450 ? 450 : gameField.Width + 310;
        //LGameOver.Margin = new Thickness(35 + (gameField.Width - LGameOver.Width) / 2, (gameField.Height / 2), 0, 0);
        //LGameOver.Visibility = Visibility.Collapsed;
    }

    ///// <summary>
    ///// Set high scores values to corresponding labels
    ///// </summary>
    ///// <param name="highScores"></param>
    private void SetHighScores(string[] highScores) {
        Label[] lHighScore = new Label[] { LHighSore1, LHighSore2, LHighSore3, LHighSore4, LHighSore5 };
        if (highScores.Length == 0) {
            int index = 1;
            foreach (Label label in lHighScore) {
                label.Content = $"{index++}. ";
            }
        }
        else {
            for (int i = 0; i < lHighScore.Length; i++) {
                if (i >= highScores.Length) lHighScore[i].Content = $"{i+1}. ";
                else { lHighScore[i].Content = highScores[i]; }
            }
        }
    }
    private void LanguageChange() {                                             // IN main window data binding does not working good
        ComboBoxItem selectedItem = CBLanguage.SelectedItem as ComboBoxItem;
        string cultureCode = selectedItem.Tag.ToString();
   
        CultureInfo culture = new CultureInfo(cultureCode);
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        BLeft.Content = TXTS.ButtonLeft;
        BRight.Content = TXTS.ButtonRight;
        BRotate.Content = TXTS.ButtonRotate;
        //LScore.Content = TXTS.LabelScore + _tetris.Score.ToString();
        MIGame.Header = TXTS.MIGame;
        MIHelp.Header = TXTS.MIHelp;
        LGameOver.Content = TXTS.LabelGameOver;
        MIEnd.Header = TXTS.MIEnd;
        MINewGame.Header = TXTS.MINewGame;
        MISettings.Header = TXTS.MISettings;
        MIOptions.Header = TXTS.MIOptions;
        MIResetScores.Header = TXTS.MIResetScores;
        MIMusic.Header = TXTS.MIMusic;
        MIHighScores.Header = TXTS.MIHighScores;
        GBTopScores.Header = TXTS.GBTopScores;
        if (_tetris.Timer != null && _tetris.Timer.IsEnabled) { BStart.Content = TXTS.ButtonStart_pause; }
        else { BStart.Content = TXTS.ButtonStart;}

    }
    private void WTetris_Loaded(object sender, RoutedEventArgs e) {                 // IN main window data binding does not working good
        BLeft.Content = TXTS.ButtonLeft;
        BRight.Content = TXTS.ButtonRight;
        BRotate.Content = TXTS.ButtonRotate;
        //LScore.Content = TXTS.LabelScore + _tetris.Score.ToString();
        MIGame.Header = TXTS.MIGame;
        MIHelp.Header = TXTS.MIHelp;
        LGameOver.Content = TXTS.LabelGameOver;
        MIEnd.Header = TXTS.MIEnd;
        MIMusic.Header = TXTS.MIMusic;
        MINewGame.Header = TXTS.MINewGame;
        MISettings.Header = TXTS.MISettings;
        MIHighScores.Header = TXTS.MIHighScores;
        if (_tetris.Timer != null && _tetris.Timer.IsEnabled) { BStart.Content = TXTS.ButtonStart_pause; }
        else { BStart.Content = TXTS.ButtonStart; }
        if (_tetris.Settings.PlayOnStartApp && MIMusic.IsChecked) { MEMusic.Play(); }
        else { MIMusic.IsChecked = false; }
    }
    private void WTetris_KeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Left) { if (_tetris.Timer != null && _tetris.Timer.IsEnabled) _tetris.MoveLeft(); }
        if (e.Key == Key.Right) { if (_tetris.Timer != null && _tetris.Timer.IsEnabled) _tetris.MoveRight(); }
        if (e.Key == Key.Up) { if (_tetris.Timer != null && _tetris.Timer.IsEnabled) _tetris.RotateShape(); }
        if (e.Key == Key.Space) {
            if (!_tetris.GameIsOn && _tetris.IsClearBoard) {
                _tetris.StartGame();
                _tetris.GameIsOn = true;
                BStart.Content = TXTS.ButtonStart_pause;
            }
            else if (_tetris.Timer != null && _tetris.Timer.IsEnabled && _tetris.GameIsOn) { _tetris.Timer.Stop(); BStart.Content = TXTS.ButtonStart; }
            else if (_tetris.Timer != null && !_tetris.Timer.IsEnabled && _tetris.GameIsOn) { _tetris.Timer.Start(); BStart.Content = TXTS.ButtonStart_pause; }
        }
        if (e.Key == Key.N) {
            if (_tetris.Timer != null && !_tetris.Timer.IsEnabled) {
                LGameOver.Visibility = Visibility.Collapsed;
                _tetris.NewGame();
                BStart.Content = TXTS.ButtonStart;
            }
        }
        if (e.Key == Key.Down) { if (_tetris.Timer != null && _tetris.Timer.IsEnabled) _tetris.MoveDown(); }
        //if (e.Key == Key.T) { MessageBox.Show(Directory.GetCurrentDirectory()); }  //test key
    }
    private void BStart_Click(object sender, RoutedEventArgs e) {
        if (!_tetris.GameIsOn && _tetris.IsClearBoard) {
            _tetris.StartGame();
            _tetris.GameIsOn = true;
            BStart.Content = TXTS.ButtonStart_pause;
        }
        else if (_tetris.Timer != null && _tetris.Timer.IsEnabled && _tetris.GameIsOn) { _tetris.Timer.Stop(); BStart.Content = TXTS.ButtonStart; }
        else if (_tetris.Timer != null && !_tetris.Timer.IsEnabled && _tetris.GameIsOn) { _tetris.Timer.Start(); BStart.Content = TXTS.ButtonStart_pause; }
    }
    private void BLeft_Click(object sender, RoutedEventArgs e) {
        if (_tetris.Timer != null && _tetris.Timer.IsEnabled) _tetris.MoveLeft();
    }
    private void BRight_Click(object sender, RoutedEventArgs e) {
        if (_tetris.Timer != null && _tetris.Timer.IsEnabled) _tetris.MoveRight();
    }
    private void BRotate_Click(object sender, RoutedEventArgs e) {
        if (_tetris.Timer != null && _tetris.Timer.IsEnabled) _tetris.RotateShape();
    }
    private void CBLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (BLeft != null) LanguageChange();
    }
    private void MINewGame_Click(object sender, RoutedEventArgs e) {
        if (_tetris.Timer != null && !_tetris.Timer.IsEnabled) {
            LGameOver.Visibility = Visibility.Collapsed;
            _tetris.NewGame();
            BStart.Content = TXTS.ButtonStart;
        }
    }
    private void MIEnd_Click(object sender, RoutedEventArgs e) {
        this.Close();
    }
    private void MISettings_Click(object sender, RoutedEventArgs e) {
        WSettings wSettings = new WSettings();
        wSettings.Owner = this;
        wSettings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        bool? result = wSettings.ShowDialog();
        if (result == true) {
            _tetris.Settings = AppSettings.LoadFromFile(settingsFile);
            if (_tetris.Timer != null) _tetris.Timer.Interval = TimeSpan.FromMilliseconds(550 - _tetris.Settings.Speed * 50); // Higher speed means run faster
            _tetris.SetUI();
            _tetris.NewGame();
            if (_tetris.Timer != null && _tetris.Timer.IsEnabled) { BStart.Content = TXTS.ButtonStart_pause; }
            else { BStart.Content = TXTS.ButtonStart; }
        }
        else {
        }      
    }
    private void MIHelp_Click(object sender, RoutedEventArgs e) {
        MessageBox.Show(WTetris, TXTS.MIHelpMessage);
    }
    private void MIMusic_Unchecked(object sender, RoutedEventArgs e) {
        MEMusic.Stop();
    }
    private void MIMusic_Checked(object sender, RoutedEventArgs e) {
        if (MEMusic != null) MEMusic.Play();
    }
    private void MIResetScores_Click(object sender, RoutedEventArgs e) {
        MessageBoxResult result = MessageBox.Show(TXTS.MBResetScores, TXTS.MBResetScoresCaption, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
        if (result == MessageBoxResult.OK) {
            highScores = new HighScores();
            SetHighScores(highScores.GetXFirstHighScoresToString(countOfHighscores));
            highScores.SaveToFile("highscores.xml");
        }
    }
    private void MEMusic_MediaEnded(object sender, RoutedEventArgs e) {
        MEMusic.Position = TimeSpan.Zero;
        MEMusic.Play();
    }
    private void WTetris_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
        highScores.SaveToFile(highScoresFile);
        _tetris.Settings.SaveToFile(settingsFile);
    }
    private void MIHighScores_Click(object sender, RoutedEventArgs e) {
        WHighScores wHighScores = new WHighScores();
        wHighScores.Owner = this;
        wHighScores.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        wHighScores.ShowDialog();
    }
}