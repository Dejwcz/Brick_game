using System.ComponentModel;

namespace Brick_game;
public class Cell : INotifyPropertyChanged {
    private bool _isOn;

    public bool IsOn {
        get => _isOn;
        set {
            if (_isOn != value) {
                _isOn = value;
                OnPropertyChanged(nameof(IsOn));
            }
        }
    }

    public Cell(bool isOn) {
        IsOn = isOn;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
