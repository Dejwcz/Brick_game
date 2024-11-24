using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static System.Formats.Asn1.AsnWriter;

namespace Brick_game;
public class Tetris {
        
    private bool _oBrick;                                //If current shape is OBrick
    private Random _rand = new Random();
    private Shapes _shapes;
    private int[] _currentShape;
    private bool _gameIsOn;
    private int _score;
    private int _sumSquares;
    private bool _isClearBoard;
    private string _highScoresFile = "highscores.xml";
    private string _settingsFile = "settings.xml";

    public ObservableCollection<Cell> Squares { get; set; }
    public AppSettings Settings { get; set; }
    public HighScores HighScores { get; set; }
  
    public bool IsClearBoard {
        get => _isClearBoard;
        set {
            if (_isClearBoard != value) {
                _isClearBoard = value;
                OnGameIsOnChanged();
            }
        }
    }
    public bool GameIsOn {
        get => _gameIsOn;
        set {
            if (_gameIsOn != value) {
                _gameIsOn = value;
                OnGameIsOnChanged();
            }
        }
    }
    public int Score {
        get => _score;
        set {
            if (_score != value) {
                _score = value;
                OnScoreChanged();
            }
        }
    }

    public event EventHandler IsClearBoardChanged;
    public event EventHandler GameIsOnChanged;
    public event EventHandler ScoreChanged;

    protected virtual void OnIsClearBoardChanged() {
        GameIsOnChanged?.Invoke(this, EventArgs.Empty);
    }
    protected virtual void OnGameIsOnChanged() {
        GameIsOnChanged?.Invoke(this, EventArgs.Empty);
    }
    protected virtual void OnScoreChanged() {
        ScoreChanged?.Invoke(this, EventArgs.Empty);
    }

    public Tetris() {
        this.Settings = AppSettings.LoadFromFile(_settingsFile);
        this.HighScores = HighScores.LoadFromFile(_highScoresFile);
        this.Score = 0;
        this.GameIsOn = false;
        this.IsClearBoard = true;
        Squares = new ObservableCollection<Cell>();
        _sumSquares = Settings.Columns * Settings.Rows;
        for (int i = 0; i < _sumSquares; i++) {
            Squares.Add(new Cell(false));
        }
    }

    public void NewGame() {                                                     //Clears board
        {
            Squares.Clear();
            for (int i = 0; i < _sumSquares; i++) { Squares.Add(new Cell(false)); }
        }
        _shapes = new Shapes(Settings.Columns);
        _currentShape = CreateNewShape();
        this.IsClearBoard = true;
        this.Score = 0;
    }

    public void MoveDown() {
        this.IsClearBoard = false;
        bool canMove = true;

        List<int> bricksToCheck = new List<int>();                                                  //Finding bottom bricks, for test of contact it needs only the bottom bricks
        foreach (int b in _currentShape) {
            bool isBottomBrick = true;
            foreach (int other in _currentShape) {
                if (other != b && other == b + Settings.Columns) { isBottomBrick = false; break; }
            }
            if (isBottomBrick) { bricksToCheck.Add(b); }
        }
        foreach (int b in bricksToCheck) {                                                            //Checking free space to move
            if (b + Settings.Columns >= Settings.Rows * Settings.Columns || Squares[b + Settings.Columns].IsOn == true) {
                canMove = false;
                switch (UpdateGameField()) {                                                            //If methods return sum of cleared rows updates score 
                    case 1: this.Score += 40; break;
                    case 2: this.Score += 100; break;
                    case 3: this.Score += 300; break;
                    case 4: this.Score += 1200; break;
                }
                break;
            }
        }
        if (canMove) {                                                                                  //Moving
            foreach (int b in _currentShape) {
                Squares[b].IsOn = false;
            }
            for (int i = 0; i < _currentShape.Length; i++) {
                _currentShape[i] += Settings.Columns;
                Squares[_currentShape[i]].IsOn = true;
            }
        }
        else {                                                                                           //If the shape is down, new starts. 
            _currentShape = CreateNewShape();
            if (!CanPlaceNewShape(_currentShape)) {                                                       //If there is no room for the new, game ends
                GameOver();
            }
        }
    }

    private void GameOver() {
        foreach (int b in _currentShape) { Squares[b].IsOn = true; }
        if (this.Score > 0) {                                        //If is any score saves it and actualizes top game scores
            EnterNewScore();
            HighScores.SaveToFile("highscores.xml");
        }
        this.GameIsOn = false;
    }

    public void MoveRight() {
        bool canMove = true;

        List<int> bricksToCheck = new List<int>();                                                  //Finding right bricks, for test of contact it needs only the right bricks
        foreach (int b in _currentShape) {
            bool isLeftBrick = true;
            foreach (int other in _currentShape) {
                if (other != b && other == b + 1) { isLeftBrick = false; break; }
            }
            if (isLeftBrick) { bricksToCheck.Add(b); }
        }
        foreach (int b in bricksToCheck) {                                                            //Checking free space to move (edge or another brick)
            if (b % Settings.Columns == Settings.Columns - 1 || Squares[b + 1].IsOn == true) {
                canMove = false;
                break;
            }
        }
        if (canMove) {                                                                                  //Moving
            foreach (int b in _currentShape) {
                Squares[b].IsOn = false;
            }
            for (int i = 0; i < _currentShape.Length; i++) {
                _currentShape[i] += 1;
                Squares[_currentShape[i]].IsOn = true;
            }
        }
    }

    public void MoveLeft() {
        bool canMove = true;

        List<int> bricksToCheck = new List<int>();                                                  //Finding left bricks, for test of contact it needs only the left bricks
        foreach (int b in _currentShape) {
            bool isLeftBrick = true;
            foreach (int other in _currentShape) {
                if (other != b && other == b - 1) { isLeftBrick = false; break; }
            }
            if (isLeftBrick) { bricksToCheck.Add(b); }
        }
        foreach (int b in bricksToCheck) {                                                            //Checking free space to move (edge or another brick)
            if (b % Settings.Columns == 0 || Squares[b - 1].IsOn == true) {
                canMove = false;
                break;
            }
        }
        if (canMove) {                                                                                  //Moving
            foreach (int b in _currentShape) {
                Squares[b].IsOn = false;
            }
            for (int i = 0; i < _currentShape.Length; i++) {
                _currentShape[i] -= 1;
                Squares[_currentShape[i]].IsOn = true;
            }
        }
    }

    public void RotateShape() {
        if (!_oBrick) {                                                           //If shape is OBrick no rotation needed
            int pivot = _currentShape[1];                                              //Second brick is selected like pivot for turning
            int pivotx = pivot % Settings.Columns;
            int pivoty = pivot / Settings.Columns;
            lock (this) {
                foreach (int b in _currentShape) {
                    if (b != pivot) {
                        int relativeX = b % Settings.Columns - pivotx;                //Getting relative coordinates
                        int relativeY = b / Settings.Columns - pivoty;
                        int rotatedX = -relativeY;                               //Rotation
                        int rotatedY = relativeX;
                        int newX = pivotx + rotatedX;                            //Getting absolute coordinates
                        int newY = pivoty + rotatedY;
                        int newPos = newY * 10 + newX;
                        if (newX < 0 || newX >= Settings.Columns || newY < 0 || newY >= Settings.Rows ||
                        (Squares[newPos].IsOn == true && !_currentShape.Any(brick => brick == newPos))) return;
                    }
                }
                int[] newShape = new int[_currentShape.Length];

                for (int i = 0; i < _currentShape.Length; i++) {
                    int original = _currentShape[i];
                    newShape[i] = original;
                }
                for (int i = 0; i < newShape.Length; i++) {
                    if (newShape[i] != pivot) {
                        int relativeX = newShape[i] % Settings.Columns - pivotx;
                        int relativeY = newShape[i] / Settings.Columns - pivoty;
                        newShape[i] = (pivoty + relativeX) * Settings.Columns + (pivotx + (-relativeY));
                    }
                }
                for (int i = 0; i < _currentShape.Length; i++) { Squares[_currentShape[i]].IsOn = false; }
                if (CanPlaceNewShape(newShape)) {
                    for (int j = 0; j < _currentShape.Length; j++) {
                        if (_currentShape[j] != pivot) {
                            int relativeX = _currentShape[j] % Settings.Columns - pivotx;
                            int relativeY = _currentShape[j] / Settings.Columns - pivoty;
                            _currentShape[j] = (pivoty + relativeX) * Settings.Columns + (pivotx + (-relativeY));
                        }
                        Squares[_currentShape[j]].IsOn = true;
                    }
                }
                else { foreach (int b in _currentShape) { Squares[b].IsOn = true; } }
            }
        }
    }

    private int[] CreateNewShape() {                                                              //Making deep copy from array of shapes for moving with copy and leaving the original unchanged
        int[] originalShape = _shapes.ShapesArray[_rand.Next(_shapes.Count)];
        int[] newShape = new int[originalShape.Length];
        _oBrick = false;

        for (int i = 0; i < originalShape.Length; i++) {
            int original = originalShape[i];
            newShape[i] = original;
        }
        if (CompareShapes(newShape, _shapes.OShape)) { _oBrick = true; }                                     //for skip rotation
        return newShape;
    }

    private bool CanPlaceNewShape(int[] shape) {                                                  //Checking space for new shape
        foreach (int b in shape) {
            if (Squares[b].IsOn == true) {
                return false;
            }
        }
        return true;
    }

    private bool CompareShapes(int[] shape1, int[] shape2) {
        if (shape1.Length != shape2.Length) { return false; }
        for (int i = 0; i < shape1.Length; i++) {
            if (!(shape1[i] == shape2[i])) { return false; }
        }
        return true;
    }

    private int UpdateGameField() {                                                                 //If is there full line on gameboard, deletes it and moves all others above down
        int rowCleared = 0;                                                                         //and returns sum of cleared rows 
        bool fullRow;
        for (int i = 0; i < Settings.Rows; i++) {
            fullRow = true;
            for (int j = 0 + Settings.Columns * i; j < Settings.Columns + Settings.Columns * i; j++) {
                if (Squares[j].IsOn == false) {                              //If finds empty field skips row
                    fullRow = false;
                    break;
                }
            }
            if (fullRow) {
                rowCleared++;
                for (int k = 0 + Settings.Columns * i; k < Settings.Columns + Settings.Columns * i; k++) { Squares[k].IsOn = false; }
                if (i == Settings.Rows - 1) {
                    for (int l = (Settings.Columns * Settings.Rows) - 1; l >= Settings.Columns; l--) { Squares[l].IsOn = Squares[l - Settings.Columns].IsOn ? true : false; }
                }
                else {
                    for (int l = (Settings.Columns + Settings.Columns * i) - 1; l >= Settings.Columns; l--) { Squares[l].IsOn = Squares[l - Settings.Columns].IsOn ? true : false; }
                }
            }
        }
        return rowCleared;
    }

    private void EnterNewScore() {                  //Opens window to enter new name and save record to highscores records
        WEnterName wEnterName = new WEnterName();
        wEnterName.Owner = Application.Current.MainWindow;
        wEnterName.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        wEnterName.ShowDialog();
        HighScores.AddRecord(new Record(wEnterName.TBEnterName.Text, Score));
        wEnterName.Close();
    }
}
