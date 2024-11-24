using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Brick_game;
public class TetrisViewModel : INotifyPropertyChanged {

    private Tetris _tetris;
    private DispatcherTimer _timer;
    private bool _isGameActive;
    private string _buttonContent;

    public RelayCommand MoveLeftCommand { get; }
    public RelayCommand MoveRightCommand { get; }
    public RelayCommand MoveDownCommand { get; }
    public RelayCommand RotateCommand { get; }
    public RelayCommand StartPauseCommand { get; }
    public RelayCommand NewGameCommand { get; }
    public RelayCommand SettingsOpenCommand { get; }
    public RelayCommand HighScoresOpenCommand { get; }

    public ObservableCollection<Cell> GameBoard => _tetris.Squares;
    public int Columns => _tetris.Settings.Columns;
    public int Rows => _tetris.Settings.Rows;
    public Brush GridColor => _tetris.Settings.GridColorBrush;
    public Brush BrickColor => _tetris.Settings.BrickColorBrush;
    public Brush BackgroundColor => _tetris.Settings.BackgroundColorBrush;

    public ObservableCollection<Record> TopScores {
        get => new ObservableCollection<Record>(HighScores.GetXFirstHighScores(5));
    }

    public string ButtonContent {
        get => _buttonContent;
        private set {
            if (_buttonContent != value) {
                _buttonContent = value;
                OnPropertyChanged(nameof(ButtonContent));
            }
        }
    }
    public bool IsGameActive {
        get => _isGameActive;
        set {
            if (_isGameActive != value) {
                _isGameActive = value;
                OnPropertyChanged(nameof(IsGameActive));
                RotateCommand.RaiseCanExecuteChanged();
                MoveDownCommand.RaiseCanExecuteChanged();
                MoveLeftCommand.RaiseCanExecuteChanged();
                MoveRightCommand.RaiseCanExecuteChanged();
                StartPauseCommand.RaiseCanExecuteChanged();
                NewGameCommand.RaiseCanExecuteChanged();
            }
        }
    }
    public bool IsClearBoard {
        get => _tetris.IsClearBoard;
        set {
            if (_tetris.IsClearBoard != value) {
                _tetris.IsClearBoard = value;
            }
        }
    }
    public bool GameIsOn {
        get => _tetris.GameIsOn;
        set {
            if (_tetris.GameIsOn != value) {
                _tetris.GameIsOn = value;
                OnPropertyChanged(nameof(GameIsOn));
                OnPropertyChanged(nameof(TopScores));
            }
        }
    }
    public int Score {
        get => _tetris.Score;
        set {
            if (_tetris.Score != value) {
                _tetris.Score = value;
                OnPropertyChanged(nameof(Score));
            }
        }
    }

    public AppSettings Settings {
        get => _tetris.Settings;
        set {
            if (_tetris.Settings != value) {
                _tetris.Settings = value;
                OnPropertyChanged(nameof(Settings));
            }
        }
    }
    public HighScores HighScores {
        get => _tetris.HighScores;
        set {
            if (_tetris.HighScores != value) {
                _tetris.HighScores = value;
                OnPropertyChanged(nameof(HighScores));
            }
        }
    }

    public TetrisViewModel(Tetris tetris) {
        _tetris = tetris;
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(550 - _tetris.Settings.Speed * 50);
        _timer.Tick += TimerTick;

        _tetris.GameIsOnChanged += Tetris_GameIsOnChanged;
        _tetris.ScoreChanged += Tetris_ScoreChanged;

        MoveLeftCommand = new RelayCommand(_ => _tetris.MoveLeft(), _ => GameIsOn && IsGameActive);
        MoveRightCommand = new RelayCommand(_ => _tetris.MoveRight(), _ => GameIsOn && IsGameActive);
        MoveDownCommand = new RelayCommand(_ => _tetris.MoveDown(), _ => GameIsOn && IsGameActive);
        RotateCommand = new RelayCommand(_ => _tetris.RotateShape(), _ => GameIsOn && IsGameActive);
        StartPauseCommand = new RelayCommand(_ => StartPauseGame());
        NewGameCommand = new RelayCommand(_ => NewGame());
        SettingsOpenCommand = new(OpenSettings);
        HighScoresOpenCommand = new(OpenHighScores);

        _tetris.GameIsOnChanged += (s, e) => CommandManager.InvalidateRequerySuggested();
        _tetris.ScoreChanged += (s, e) => CommandManager.InvalidateRequerySuggested();
        _tetris.IsClearBoardChanged += (s, e) => CommandManager.InvalidateRequerySuggested();

        UpdatebNewGame();
    }

    private void OpenHighScores(object obj) {
        var mainWindow = obj as Window;
        WHighScores wHighScores = new WHighScores();
        wHighScores.Owner = mainWindow;
        wHighScores.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        wHighScores.ShowDialog();
    }

    private void OpenSettings(object obj) {
        var mainWindow = obj as Window;
        WSettings wSettings = new WSettings();
        wSettings.Owner = mainWindow;
        wSettings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        wSettings.ShowDialog();
    }

    private void NewGame() {
        IsGameActive = false;
        _timer.Stop();
        _tetris.NewGame();
    }

    private void StartPauseGame() {
        if (!IsGameActive) {
            StartGame();
        }
        else {
            PauseGame();
        }
        UpdatebNewGame();
    }

    private void PauseGame() {
        _timer.Stop();
        IsGameActive = false;
    }

    private void StartGame() {
        if (!GameIsOn) {
            _tetris.NewGame();
            _tetris.GameIsOn = true;
        }
        _timer.Start();
        IsGameActive = true;
    }

    private void TimerTick(object? sender, EventArgs e) {
        _tetris.MoveDown();
        if (!GameIsOn) {
            _timer.Stop();
            IsGameActive = false;
        }
    }

    public void UpdatebNewGame() {
        if (!GameIsOn) {
            ButtonContent = (string)Application.Current.Resources["ButtonNewGame"];
        }
        else {
            ButtonContent = IsGameActive
                ? (string)Application.Current.Resources["ButtonPause"]
                : (string)Application.Current.Resources["ButtonStart"];
        }
    }

    private void Tetris_GameIsOnChanged(object sender, EventArgs e) {
        OnPropertyChanged(nameof(GameIsOn));
        UpdatebNewGame();
    }

    private void Tetris_ScoreChanged(object sender, EventArgs e) {
        OnPropertyChanged(nameof(Score));
    }

    private void Tetris_IsClearBoardChanged(object sender, EventArgs e) {
        OnPropertyChanged(nameof(IsClearBoard));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
