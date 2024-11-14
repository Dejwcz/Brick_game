using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static System.Formats.Asn1.AsnWriter;

namespace Brick_game;
public class Tetris {
        
    private bool oBrick;                                //If current shape is OBrick
    private Random rand = new Random();
    private Shapes shapes;

    private Canvas[] squares;
    private Grid gameField;

    private int[] currentShape;


    public bool IsClearBoard { get; set; }                   //Game board is cleared
    private bool _gameIsOn;
    public bool GameIsOn {
        get => _gameIsOn;
        set {
            if (_gameIsOn != value) {
                _gameIsOn = value;
                OnGameIsOnChanged();
            }
        }
    }

    public event EventHandler GameIsOnChanged;

    protected virtual void OnGameIsOnChanged() {
        GameIsOnChanged?.Invoke(this, EventArgs.Empty);
    }
    private int _score;

    public int Score {
        get => _score;
        set {
            if (_score != value) {
                _score = value;
                OnScoreChanged();
            }
        }
    }

    public event EventHandler ScoreChanged;

    protected virtual void OnScoreChanged() {
        ScoreChanged?.Invoke(this, EventArgs.Empty);
    }
    public DispatcherTimer Timer { get; set; }
    public StackPanel MainContainer { get; set; }
    public AppSettings Settings { get; set; }
    public HighScores HighScores { get; set; }

    private string highScoresFile = "highscores.xml";
    private string settingsFile = "settings.xml";

    public Tetris(StackPanel mainContainer, DispatcherTimer timer) {
        this.MainContainer = mainContainer;
        this.Timer = timer;
        this.Settings = AppSettings.LoadFromFile(settingsFile);
        this.Score = 0;
        this.GameIsOn = false;
        this.MainContainer.Children.Clear();
        this.IsClearBoard = true;
    }

    public void SetUI() {
        #region gameField
        gameField = new Grid {
            Width = Settings.Columns * (Settings.SquareSize + 2),
            Height = Settings.Rows * (Settings.SquareSize + 2),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top
        };

        for (int i = 0; i < Settings.Rows; i++) { gameField.RowDefinitions.Add(new RowDefinition()); }
        for (int i = 0; i < Settings.Columns; i++) { gameField.ColumnDefinitions.Add(new ColumnDefinition()); }
        squares = new Canvas[Settings.Rows * Settings.Columns];
        for (int i = 0; i < Settings.Rows * Settings.Columns; i++) {
            squares[i] = new Canvas { Height = Settings.SquareSize, Width = Settings.SquareSize, Background = Settings.GetBackgroundColor() };

            int row = i / Settings.Columns;                  //Row position determination
            int col = i % Settings.Columns;                  //Column position determination

            Border border = new Border {
                BorderBrush = Settings.GetGridColor(),   // Line color
                BorderThickness = new Thickness(1), // 
                Child = squares[i]                  // Adding square into borders
            };

            Grid.SetRow(border, row);               //
            Grid.SetColumn(border, col);            //Set row and column
            gameField.Children.Add(border);         //Adding square with borders into grid
        }
        #endregion
        #region WTetrisAndItemsPropertiesSettings
        this.MainContainer.Children.Add(gameField);
        this.MainContainer.Height = gameField.Height;
        this.MainContainer.Width = gameField.Width;

        //WTetris.ResizeMode = ResizeMode.NoResize;
        //WTetris.Height = gameField.Height + 120 < 480 ? 480 : gameField.Height + 120;
        //WTetris.Width = gameField.Width + 310 < 450 ? 450 : gameField.Width + 310;
        //LGameOver.Margin = new Thickness(35 + (gameField.Width - LGameOver.Width) / 2, (gameField.Height / 2), 0, 0);
        //LGameOver.Visibility = Visibility.Collapsed;
        #endregion

        shapes = new Shapes(Settings.Columns);
        currentShape = CreateNewShape();
    }
    public void StartGame() {                                                      //Start game    
        if (this.Timer == null) {
            this.Timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(550 - Settings.Speed * 50) };
            this.Timer.Tick += (s, e) => MoveDown();
        }
        this.Timer.Start();

    }
    public void NewGame() {                                                     //Clears board
        {
            foreach (Canvas b in squares) { b.Background = Settings.GetBackgroundColor(); }
        }
        this.IsClearBoard = true;
        this.Score = 0;
        //LScore.Content = TXTS.LabelScore + score.ToString();                 //up to window
    }
    public void MoveDown() {
        this.IsClearBoard = false;
        bool canMove = true;

        List<int> bricksToCheck = new List<int>();                                                  //Finding bottom bricks, for test of contact it needs only the bottom bricks
        foreach (int b in currentShape) {
            bool isBottomBrick = true;
            foreach (int other in currentShape) {
                if (other != b && other == b + Settings.Columns) { isBottomBrick = false; break; }
            }
            if (isBottomBrick) { bricksToCheck.Add(b); }
        }
        foreach (int b in bricksToCheck) {                                                            //Checking free space to move
            if (b + Settings.Columns >= Settings.Rows * Settings.Columns || squares[b + Settings.Columns].Background.ToString() == Settings.BrickColor) {
                canMove = false;
                switch (UpdateGameField()) {                                                            //If methods return sum of cleared rows updates score 
                    case 1: this.Score += 40; break;
                    case 2: this.Score += 100; break;
                    case 3: this.Score += 300; break;
                    case 4: this.Score += 1200; break;
                }
                //LScore.Content = TXTS.LabelScore + score.ToString();
                break;
            }
        }
        if (canMove) {                                                                                  //Moving
            foreach (int b in currentShape) {
                squares[b].Background = Settings.GetBackgroundColor();
            }
            for (int i = 0; i < currentShape.Length; i++) {
                currentShape[i] += Settings.Columns;
                squares[currentShape[i]].Background = Settings.GetBrickColor();
            }
        }
        else {                                                                                           //If the shape is down, new starts. 
            currentShape = CreateNewShape();
            if (!CanPlaceNewShape(currentShape)) {                                                       //If there is no room for the new, game ends
                GameOver();
            }
        }
    }
    private void GameOver() {
        foreach (int b in currentShape) { squares[b].Background = Settings.GetBrickColor(); }
        this.Timer.Stop();
        this.GameIsOn = false;
        //LGameOver.Visibility = Visibility.Visible;              //Shows game over
        if (this.Score > 0) {                                        //If is any score saves it and actualizes top game scores
            EnterNewScore();
            HighScores.SaveToFile("highscores.xml");
        }
        else { ; }
    }
    private void EnterNewScore() {                  //Opens window to enter new name and save record to highscores records
        WEnterName wEnterName = new WEnterName();
        wEnterName.Owner = Application.Current.MainWindow;
        wEnterName.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        wEnterName.ShowDialog();
        HighScores.AddRecord(new Record(wEnterName.TBEnterName.Text, Score));
        wEnterName.Close();
    }
    public void MoveRight() {
        bool canMove = true;

        List<int> bricksToCheck = new List<int>();                                                  //Finding right bricks, for test of contact it needs only the right bricks
        foreach (int b in currentShape) {
            bool isLeftBrick = true;
            foreach (int other in currentShape) {
                if (other != b && other == b + 1) { isLeftBrick = false; break; }
            }
            if (isLeftBrick) { bricksToCheck.Add(b); }
        }
        foreach (int b in bricksToCheck) {                                                            //Checking free space to move (edge or another brick)
            if (b % Settings.Columns == Settings.Columns - 1 || squares[b + 1].Background.ToString() == Settings.BrickColor) {
                canMove = false;
                break;
            }
        }
        if (canMove) {                                                                                  //Moving
            foreach (int b in currentShape) {
                squares[b].Background = Settings.GetBackgroundColor();
            }
            for (int i = 0; i < currentShape.Length; i++) {
                currentShape[i] += 1;
                squares[currentShape[i]].Background = Settings.GetBrickColor();
            }
        }
    }
    public void MoveLeft() {
        bool canMove = true;

        List<int> bricksToCheck = new List<int>();                                                  //Finding left bricks, for test of contact it needs only the left bricks
        foreach (int b in currentShape) {
            bool isLeftBrick = true;
            foreach (int other in currentShape) {
                if (other != b && other == b - 1) { isLeftBrick = false; break; }
            }
            if (isLeftBrick) { bricksToCheck.Add(b); }
        }
        foreach (int b in bricksToCheck) {                                                            //Checking free space to move (edge or another brick)
            if (b % Settings.Columns == 0 || squares[b - 1].Background.ToString() == Settings.BrickColor) {
                canMove = false;
                break;
            }
        }
        if (canMove) {                                                                                  //Moving
            foreach (int b in currentShape) {
                squares[b].Background = Settings.GetBackgroundColor();
            }
            for (int i = 0; i < currentShape.Length; i++) {
                currentShape[i] -= 1;
                squares[currentShape[i]].Background = Settings.GetBrickColor();
            }
        }
    }
    public void RotateShape() {
        if (!oBrick) {                                                           //If shape is OBrick no rotation needed
            int pivot = currentShape[1];                                              //Second brick is selected like pivot for turning
            int pivotx = pivot % Settings.Columns;
            int pivoty = pivot / Settings.Columns;
            lock (this) {
                foreach (int b in currentShape) {
                    if (b != pivot) {
                        int relativeX = b % Settings.Columns - pivotx;                //Getting relative coordinates
                        int relativeY = b / Settings.Columns - pivoty;
                        int rotatedX = -relativeY;                               //Rotation
                        int rotatedY = relativeX;
                        int newX = pivotx + rotatedX;                            //Getting absolute coordinates
                        int newY = pivoty + rotatedY;
                        int newPos = newY * 10 + newX;
                        if (newX < 0 || newX >= Settings.Columns || newY < 0 || newY >= Settings.Rows ||
                        (squares[newPos].Background.ToString() == Settings.BrickColor && !currentShape.Any(brick => brick == newPos))) return;
                    }
                }
                int[] newShape = new int[currentShape.Length];

                for (int i = 0; i < currentShape.Length; i++) {
                    int original = currentShape[i];
                    newShape[i] = original;
                }
                for (int i = 0; i < newShape.Length; i++) {
                    if (newShape[i] != pivot) {
                        int relativeX = newShape[i] % Settings.Columns - pivotx;
                        int relativeY = newShape[i] / Settings.Columns - pivoty;
                        newShape[i] = (pivoty + relativeX) * Settings.Columns + (pivotx + (-relativeY));
                    }
                }
                for (int i = 0; i < currentShape.Length; i++) { squares[currentShape[i]].Background = Settings.GetBackgroundColor(); }
                if (CanPlaceNewShape(newShape)) {
                    for (int j = 0; j < currentShape.Length; j++) {
                        if (currentShape[j] != pivot) {
                            int relativeX = currentShape[j] % Settings.Columns - pivotx;
                            int relativeY = currentShape[j] / Settings.Columns - pivoty;
                            currentShape[j] = (pivoty + relativeX) * Settings.Columns + (pivotx + (-relativeY));
                        }
                        squares[currentShape[j]].Background = Settings.GetBrickColor();
                    }
                }
                else { foreach (int b in currentShape) { squares[b].Background = Settings.GetBrickColor(); } }
            }
        }
    }
    private bool CanPlaceNewShape(int[] shape) {                                                  //Checking space for new shape
        foreach (int b in shape) {
            if (squares[b].Background.ToString() == Settings.BrickColor) {
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
    public int[] CreateNewShape() {                                                              //Making deep copy from array of shapes for moving with copy and leaving the original unchanged
        int[] originalShape = shapes.ShapesArray[rand.Next(shapes.Count)];
        int[] newShape = new int[originalShape.Length];
        oBrick = false;

        for (int i = 0; i < originalShape.Length; i++) {
            int original = originalShape[i];
            newShape[i] = original;
        }
        if (CompareShapes(newShape, shapes.OShape)) { oBrick = true; }                                     //for skip rotation
        return newShape;
    }
    private int UpdateGameField() {                                                                 //If is there full line on gameboard, deletes it and moves all others above down
        int rowCleared = 0;                                                                         //and returns sum of cleared rows 
        bool fullRow;
        for (int i = 0; i < Settings.Rows; i++) {
            fullRow = true;
            for (int j = 0 + Settings.Columns * i; j < Settings.Columns + Settings.Columns * i; j++) {
                if (squares[j].Background.ToString() == Settings.BackgroundColor) {                              //If finds empty field skips row
                    fullRow = false;
                    break;
                }
            }
            if (fullRow) {
                rowCleared++;
                for (int k = 0 + Settings.Columns * i; k < Settings.Columns + Settings.Columns * i; k++) { squares[k].Background = Settings.GetBackgroundColor(); }
                if (i == Settings.Rows - 1) {
                    for (int l = (Settings.Columns * Settings.Rows) - 1; l >= Settings.Columns; l--) { squares[l].Background = squares[l - Settings.Columns].Background; }
                }
                else {
                    for (int l = (Settings.Columns + Settings.Columns * i) - 1; l >= Settings.Columns; l--) { squares[l].Background = squares[l - Settings.Columns].Background; }
                }
            }
        }
        return rowCleared;
    }
}
