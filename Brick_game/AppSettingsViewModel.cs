using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Brick_game;

/// <summary>
/// Wrapping class for AppSetting due to implement INotifyPropertyChanged, 
/// because original class is simple as needed for storage data and serializing to xml 
/// </summary>
public class AppSettingsViewModel : INotifyPropertyChanged {
    private AppSettings appSettings;

    public AppSettingsViewModel(AppSettings appSettings) {
        this.appSettings = appSettings;
    }

    public int Rows {
        get => appSettings.Rows;
        set {
            if (appSettings.Rows != value) {
                appSettings.Rows = value;
                OnPropertyChanged(nameof(Rows));
            }
        }
    }

    public int Columns {
        get => appSettings.Columns;
        set {
            if (appSettings.Columns != value) {
                appSettings.Columns = value;
                OnPropertyChanged(nameof(Columns));
            }
        }
    }

    public int SquareSize {
        get => appSettings.SquareSize;
        set {
            if (appSettings.SquareSize != value) {
                appSettings.SquareSize = value;
                OnPropertyChanged(nameof(SquareSize));
            }
        }
    }

    public int Speed {
        get => appSettings.Speed;
        set {
            if (appSettings.Speed != value) {
                appSettings.Speed = value;
                OnPropertyChanged(nameof(Speed));
            }
        }
    }

    public string BackgroundColor {
        get => appSettings.BackgroundColor;
        set {
            if (appSettings.BackgroundColor != value) {
                appSettings.BackgroundColor = value;
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }
    }

    public string BrickColor {
        get => appSettings.BrickColor;
        set {
            if (appSettings.BrickColor != value) {
                appSettings.BrickColor = value;
                OnPropertyChanged(nameof(BrickColor));
            }
        }
    }

    public string GridColor {
        get => appSettings.GridColor;
        set {
            if (appSettings.GridColor != value) {
                appSettings.GridColor = value;
                OnPropertyChanged(nameof(GridColor));
            }
        }
    }

    public bool PlayOnStartApp {
        get => appSettings.PlayOnStartApp;
        set {
            if (appSettings.PlayOnStartApp != value) {
                appSettings.PlayOnStartApp = value;
                OnPropertyChanged(nameof(PlayOnStartApp));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}