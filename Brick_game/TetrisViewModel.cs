using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;

namespace Brick_game;
public class TetrisViewModel : INotifyPropertyChanged {
    private Tetris _tetris;

    public TetrisViewModel(Tetris tetris) {
        _tetris = tetris;
        _tetris.GameIsOnChanged += Tetris_GameIsOnChanged;
        _tetris.ScoreChanged += Tetris_ScoreChanged;
    }
    private void Tetris_GameIsOnChanged(object sender, EventArgs e) {
        OnPropertyChanged(nameof(GameIsOn));
        OnPropertyChanged(nameof(LabelVisibility));
    }
    private void Tetris_ScoreChanged(object sender, EventArgs e) {
        OnPropertyChanged(nameof(Score));
    }

    public bool IsClearBoard {
        get => _tetris.IsClearBoard;
        set {
            if (_tetris.IsClearBoard != value) {
                _tetris.IsClearBoard = value;
                OnPropertyChanged(nameof(IsClearBoard));
            }
        }
    }

    public bool GameIsOn {
        get => _tetris.GameIsOn;
        set {
            if (_tetris.GameIsOn != value) {
                _tetris.GameIsOn = value;
                //OnPropertyChanged(nameof(GameIsOn));
                //OnPropertyChanged(nameof(LabelVisibility));
            }
        }
    }

    public int Score {
        get => _tetris.Score;
        set {
            if (_tetris.Score != value) {
                _tetris.Score = value;
                //OnPropertyChanged(nameof(Score));
            }
        }
    }

    public DispatcherTimer Timer {
        get => _tetris.Timer;
        set {
            if (_tetris.Timer != value) {
                _tetris.Timer = value;
                OnPropertyChanged(nameof(Timer));
            }
        }
    }

    public StackPanel MainContainer {
        get => _tetris.MainContainer;
        set {
            if (_tetris.MainContainer != value) {
                _tetris.MainContainer = value;
                OnPropertyChanged(nameof(MainContainer));
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

    public Visibility LabelVisibility => GameIsOn ?  Visibility.Collapsed : Visibility.Visible;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
