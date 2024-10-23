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
using TXTS = Tetris.Properites.Resources;
using System.IO;

namespace Tetris; 
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    private int rows;// = 20;
    private int columns;// = 10;
    private int squareSize;// = 20;
    private int speed;// = 200;                         //Speed of falling in "miliseconds"

    private int score = 0;                              //Score
    private bool gameIsOn = false;                      //Game is running
    private bool isFalling = false;                     //Brick is falling
    private bool isClearBoard = true;                   //Game board is cleared
    private bool oBrick;                                //Current shape is OBrick
    private Random rand = new Random();
    private Brick[] LShape;
    private Brick[] JShape;
    private Brick[] IShape;
    private Brick[] OShape;
    private Brick[] SShape;
    private Brick[] TShape;
    private Brick[] ZShape;
    private Canvas[] squares;
    private Grid gameField;
    private Brick[][] shapes;
    private Brick[] currentShape;
    private DispatcherTimer timer;
    private string currentDirectory = Directory.GetCurrentDirectory();
    private string settingFile = Directory.GetCurrentDirectory() + "\\text.txt";

    public MainWindow() {
        InitializeComponent();
        SetSettings(LoadSetting());
        SetUI();
        //for (int i = 120; i < 120 + columns; i++) { squares[i].Background = Brushes.Red; }      //Test line
 
    }

    private void SetUI() {
        #region gameField
        MainContainer.Children.Clear();
        gameField = new Grid {
            Width = columns * (squareSize + 2),
            Height = rows * (squareSize + 2),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top
        };


        for (int i = 0; i < rows; i++) { gameField.RowDefinitions.Add(new RowDefinition()); }
        for (int i = 0; i < columns; i++) { gameField.ColumnDefinitions.Add(new ColumnDefinition()); }
        squares = new Canvas[rows * columns];
        for (int i = 0; i < rows * columns; i++) {
            squares[i] = new Canvas { Height = squareSize, Width = squareSize, Background = Brushes.AliceBlue };

            int row = i / columns;                  //Row position determination
            int col = i % columns;                  //Column position determination

            Border border = new Border {
                BorderBrush = Brushes.LightGreen,   // Line color
                BorderThickness = new Thickness(1), // 
                Child = squares[i]                  // Adding square into borders
            };

            Grid.SetRow(border, row);               //
            Grid.SetColumn(border, col);            //Set row and column
            gameField.Children.Add(border);         //Adding square with borders into grid
        }

        #endregion
        #region WTetrisAndItemsProperiesSettings
        MainContainer.Children.Add(gameField);
        MainContainer.Height = gameField.Height;
        MainContainer.Width = gameField.Width;
        WTetris.ResizeMode = ResizeMode.NoResize;
        WTetris.Height = gameField.Height + 120 < 300 ? 300 : gameField.Height + 120;
        WTetris.Width = gameField.Width + 310 < 450 ? 450 : gameField.Width + 310;
        LGameOver.Margin = new Thickness(35 + (gameField.Width - LGameOver.Width) / 2, (gameField.Height / 2), 0, 0);
        LGameOver.Visibility = Visibility.Collapsed;
        #endregion
        #region shapes
        //Second brick in brick array is pivot for turning around
        int center = columns / 2;

        Brick IS1 = new Brick(center, squares[center]);
        Brick IS2 = new Brick(center + columns, squares[center + columns]);
        Brick IS3 = new Brick(center + columns * 2, squares[center + columns * 2]);
        Brick IS4 = new Brick(center + columns * 3, squares[center + columns * 3]);
        IShape = new Brick[] { IS4, IS3, IS2, IS1 };

        Brick JS1 = new(center, squares[center]);
        Brick JS2 = new(center + columns, squares[center + columns]);
        Brick JS3 = new(center + columns - 1, squares[center + columns - 1]);
        Brick JS4 = new(center + columns - 2, squares[center + columns - 2]);
        JShape = new Brick[] { JS1, JS2, JS3, JS4 };

        Brick LS1 = new(center, squares[center]);
        Brick LS2 = new(center + columns, squares[center + columns]);
        Brick LS3 = new(center + columns + 1, squares[center + columns + 1]);
        Brick LS4 = new(center + columns + 2, squares[center + columns + 2]);
        LShape = new Brick[] { LS1, LS2, LS3, LS4 };

        Brick OS1 = new(center, squares[center]);
        Brick OS2 = new(center + 1, squares[center + 1]);
        Brick OS3 = new(center + columns, squares[columns]);
        Brick OS4 = new(center + columns + 1, squares[center + columns + 1]);
        OShape = new Brick[] { OS1, OS2, OS3, OS4 };

        Brick SS1 = new(center, squares[center]);
        Brick SS2 = new(center + 1, squares[center + 1]);
        Brick SS3 = new(center + columns, squares[center + columns]);
        Brick SS4 = new(center + columns - 1, squares[center + columns - 1]);
        SShape = new Brick[] { SS2, SS1, SS3, SS4 };

        Brick TS1 = new(center, squares[center]);
        Brick TS2 = new(center + columns, squares[center + columns]);
        Brick TS3 = new(center + columns + 1, squares[center + columns + 1]);
        Brick TS4 = new(center + columns - 1, squares[center + columns - 1]);
        TShape = new Brick[] { TS1, TS2, TS3, TS4 };

        Brick ZS1 = new(center, squares[center]);
        Brick ZS2 = new(center + 1, squares[center + 1]);
        Brick ZS3 = new(center + columns + 1, squares[center + columns + 1]);
        Brick ZS4 = new(center + columns + 2, squares[center + columns + 2]);
        ZShape = new Brick[] { ZS1, ZS2, ZS3, ZS4 };

        shapes = new[] { IShape, JShape, LShape, OShape, SShape, TShape, ZShape };
        currentShape = CreateNewShape();

        #endregion
    }

    public void GameOn() {                                                      //Start game    
        if (timer == null) {
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(speed) };
            timer.Tick += (s, e) => MoveDown(currentShape);
        }
        timer.Start();
    }
    public void NewGame() {                                                     //Clears board
        {
            foreach (Canvas b in squares) { b.Background = Brushes.AliceBlue; }
        }
        isClearBoard = true;
        score = 0;
        LScore.Content = TXTS.LabelScore + score.ToString();
    }
    private void MoveDown(Brick[] shape) {
        isClearBoard = false; 
        bool canMove = true;

        List<Brick> bricksToCheck = new List<Brick>();                                                  //Finding bottom bricks, for test of contact it needs only the bottom bricks
        foreach (Brick b in shape) {                                                                    
            bool isBottomBrick = true;
            foreach (Brick other in shape) {
                if (other != b && other.Pos == b.Pos + columns) { isBottomBrick = false; break; }
            }
            if (isBottomBrick) { bricksToCheck.Add(b); }
        }
        foreach (Brick b in bricksToCheck) {                                                            //Checking free space to move
            if (b.Pos + columns >= rows * columns || squares[b.Pos + columns].Background.Equals(Brushes.Red)) {
                canMove = false;
                switch (UpdateGameField()) {                                                            //If methods return sum of cleared rows updates score 
                case 1: score += 40; break;
                case 2: score += 100; break;
                case 3: score += 300; break;
                case 4: score += 1200; break;
                }
                LScore.Content = TXTS.LabelScore + score.ToString();
                break;              
            }
        }
        if (canMove) {                                                                                  //Moving
            foreach (Brick b in shape) {
                squares[b.Pos].Background = Brushes.AliceBlue;
            }
            foreach (Brick b in shape) {
                b.Pos += columns;
                squares[b.Pos].Background = Brushes.Red;
            }
        }
        else {                                                                                           //If the shape is down, new starts. 
            currentShape = CreateNewShape();
            if (!CanPlaceNewShape(currentShape)) {                                                       //If there is no room for the new, game ends
                foreach (Brick b in shape) { b.IsOn = true; }
                timer.Stop();
                gameIsOn = false;
                LGameOver.Visibility = Visibility.Visible;
            }
        }
    }
    private void MoveRight(Brick[] shape) {
        bool canMove = true;

        List<Brick> bricksToCheck = new List<Brick>();                                                  //Finding right bricks, for test of contact it needs only the right bricks
        foreach (Brick b in shape) {                                                                    
            bool isLeftBrick = true;
            foreach (Brick other in shape) {
                if (other != b && other.Pos == b.Pos + 1) { isLeftBrick = false; break; }
            }
            if (isLeftBrick) { bricksToCheck.Add(b); }
        }
        foreach (Brick b in bricksToCheck) {                                                            //Checking free space to move (edge or another brick)
            if (b.Pos % columns == columns-1 || squares[b.Pos + 1].Background.Equals(Brushes.Red)) {
                canMove = false;
                break;
            }
        }
        if (canMove) {                                                                                  //Moving
            foreach (Brick b in shape) {
                squares[b.Pos].Background = Brushes.AliceBlue;
            }
            foreach (Brick b in shape) {
                b.Pos += 1;
                squares[b.Pos].Background = Brushes.Red;
            }
        }
    }
    private void MoveLeft(Brick[] shape) {
        bool canMove = true;

        List<Brick> bricksToCheck = new List<Brick>();                                                  //Finding left bricks, for test of contact it needs only the left bricks
        foreach (Brick b in shape) {                                                                    
            bool isLeftBrick = true;
            foreach (Brick other in shape) {
                if (other != b && other.Pos == b.Pos - 1) { isLeftBrick = false; break; }
            }
            if (isLeftBrick) { bricksToCheck.Add(b); }
        }
        foreach (Brick b in bricksToCheck) {                                                            //Checking free space to move (edge or another brick)
            if (b.Pos % columns == 0 || squares[b.Pos - 1].Background.Equals(Brushes.Red)) {
                canMove = false;
                break;
            }
        }
        if (canMove) {                                                                                  //Moving
            foreach (Brick b in shape) {
                squares[b.Pos].Background = Brushes.AliceBlue;
            }
            foreach (Brick b in shape) {
                b.Pos -= 1;
                squares[b.Pos].Background = Brushes.Red;
            }
        }
    }
    private void RotateShape(Brick[] shape) {
        if (!oBrick) {                                                           //If shape is OBrick no rotation needed
            Brick pivot = shape[1];                                              //Second brick is selected like pivot for turning
            int pivotx = pivot.Pos % columns;
            int pivoty = pivot.Pos / columns;
            lock (this) {
                foreach (Brick b in shape) {
                    if (b != pivot) {
                        int relativeX = b.Pos % columns - pivotx;                //Getting relative coordinates
                        int relativeY = b.Pos / columns - pivoty;
                        int rotatedX = -relativeY;                               //Rotation
                        int rotatedY = relativeX;
                        int newX = pivotx + rotatedX;                            //Getting absolute coordinates
                        int newY = pivoty + rotatedY;
                        int newPos = newY * 10 + newX;
                        if (newX < 0 || newX >= columns || newY < 0 || newY >= rows ||
                        (squares[newPos].Background.Equals(Brushes.Red) && !shape.Any(brick => brick.Pos == newPos))) return;
                    }
                }
                Brick[] newShape = new Brick[shape.Length];

                for (int i = 0; i < shape.Length; i++) {
                    Brick original = shape[i];
                    newShape[i] = new Brick(original.Pos, squares[original.Pos]);
                    newShape[i].Check = original.Check;
                    //newShape[i].IsOn = true;
                }
                foreach (Brick b in newShape) {
                    if (b != pivot) {
                        int relativeX = b.Pos % columns - pivotx;
                        int relativeY = b.Pos / columns - pivoty;
                        b.Pos = (pivoty + relativeX) * columns + (pivotx + (-relativeY));
                    }
                }
                foreach (Brick b in shape) { squares[b.Pos].Background = Brushes.AliceBlue; }
                if (CanPlaceNewShape(newShape)) {
                    foreach (Brick b in shape) {
                        if (b != pivot) {
                            int relativeX = b.Pos % columns - pivotx;
                            int relativeY = b.Pos / columns - pivoty;
                            b.Pos = (pivoty + relativeX) * columns + (pivotx + (-relativeY));
                        }
                        squares[b.Pos].Background = Brushes.Red;
                    }
                }
                else { foreach (Brick b in shape) { squares[b.Pos].Background = Brushes.Red; } }
            }
        }
    }
    private bool CanPlaceNewShape(Brick[] shape) {                                                  //Checking space for new shape
        foreach (Brick b in shape) {
            if (squares[b.Pos].Background.Equals(Brushes.Red)) {
                return false;
            }
        }
        return true;
    }
    private bool CompareShapes(Brick[] shape1, Brick[] shape2) {
        if (shape1.Length != shape2.Length) {return false; }
        for (int i = 0; i < shape1.Length; i++) {
            if (!(shape1[i].Pos == shape2[i].Pos)) { return false; }
        }
        return true;
    }
    private Brick[] CreateNewShape() {                                                              //Making deep copy from array of shapes for moving with copy and leaving the original unchanged
        Brick[] originalShape = shapes[rand.Next(shapes.Length)];
        Brick[] newShape = new Brick[originalShape.Length];
        oBrick = false;

        for (int i = 0; i < originalShape.Length; i++) {
            Brick original = originalShape[i];
            newShape[i] = new Brick(original.Pos, squares[original.Pos]);
            newShape[i].Check = original.Check;
            //newShape[i].IsOn = true;
        }
        if (CompareShapes(newShape, OShape)) { oBrick = true; }                                     //for skip rotation
        return newShape;
    }
    private int UpdateGameField() {                                                                 //If is there full line on gameboard, deletes it and moves all others above down
        int rowCleared = 0;                                                                         //and returns sum of cleared rows 
        bool fullRow;
        for (int i = 0; i < rows; i++) {
        fullRow = true;
            for (int j = 0 + columns * i; j < columns + columns * i; j++) {
                if (squares[j].Background.Equals(Brushes.AliceBlue)) {                              //If finds empty field skips row
                    fullRow = false;
                    break;
                }
            }
            if (fullRow) {
                rowCleared++;
                for (int k = 0 + columns * i; k < columns + columns * i; k++) { squares[k].Background = Brushes.AliceBlue; }
                if (i == rows - 1) {
                    for (int l = (columns * rows) - 1; l >= columns; l--) { squares[l].Background = squares[l - columns].Background; }
                }
                else {
                    for (int l = (columns + columns * i) - 1; l >= columns; l--) { squares[l].Background = squares[l - columns].Background; }
                }
            }
        }
        return rowCleared;
    }
    private void LanguageChange() {
        ComboBoxItem selectedItem = CBLanguage.SelectedItem as ComboBoxItem;
        string cultureCode = selectedItem.Tag.ToString();
   
        CultureInfo culture = new CultureInfo(cultureCode);
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        BLeft.Content = TXTS.ButtonLeft;
        BRight.Content = TXTS.ButtonRight;
        BRotate.Content = TXTS.ButtonRotate;
        LScore.Content = TXTS.LabelScore + score.ToString();
        MIGame.Header = TXTS.MIGame;
        MIHelp.Header = TXTS.MIHelp;
        LGameOver.Content = TXTS.LabelGameOver;
        MIEnd.Header = TXTS.MIEnd;
        MINewGame.Header = TXTS.MINewGame;
        MISettings.Header = TXTS.MISettings;
        if (timer != null && timer.IsEnabled) { BStart.Content = TXTS.ButtonStart_pause; }
        else { BStart.Content = TXTS.ButtonStart;}
    }
    private void BStart_Click(object sender, RoutedEventArgs e) {
        if (!gameIsOn && isClearBoard) {
            GameOn();
            gameIsOn = true;
            BStart.Content = TXTS.ButtonStart_pause;
        }
        else if (timer != null && timer.IsEnabled && gameIsOn) { timer.Stop(); BStart.Content = TXTS.ButtonStart; }
        else if (timer != null && !timer.IsEnabled && gameIsOn) { timer.Start(); BStart.Content = TXTS.ButtonStart_pause; }
    }
    private void BLeft_Click(object sender, RoutedEventArgs e) {
        if (timer != null && timer.IsEnabled) MoveLeft(currentShape); 
    }
    private void BRight_Click(object sender, RoutedEventArgs e) {
        if (timer != null && timer.IsEnabled) MoveRight(currentShape);
    }
    private void WTetris_KeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Left) { if (timer != null && timer.IsEnabled) MoveLeft(currentShape); }
        if (e.Key == Key.Right) { if (timer != null && timer.IsEnabled) MoveRight(currentShape); }
        if (e.Key == Key.Up) { if (timer != null && timer.IsEnabled) RotateShape(currentShape); }
        if (e.Key == Key.Space) {
            if (!gameIsOn && isClearBoard) {
                GameOn();
                gameIsOn = true;
                BStart.Content = TXTS.ButtonStart_pause;
            }
            else if (timer != null && timer.IsEnabled && gameIsOn) { timer.Stop(); BStart.Content = TXTS.ButtonStart; }
            else if (timer != null && !timer.IsEnabled && gameIsOn) { timer.Start(); BStart.Content = TXTS.ButtonStart_pause; }
        }
        if (e.Key == Key.N) { NewGame(); currentShape = CreateNewShape(); }
        if (e.Key == Key.Down) { if (timer != null && timer.IsEnabled) MoveDown(currentShape); }
        //if (e.Key == Key.T) { MessageBox.Show(Directory.GetCurrentDirectory()); }  //test key
    }
    private void BRotate_Click(object sender, RoutedEventArgs e) {
        if (timer != null && timer.IsEnabled) RotateShape(currentShape);
    }
    private void WTetris_Loaded(object sender, RoutedEventArgs e) {
        BLeft.Content = TXTS.ButtonLeft;
        BRight.Content = TXTS.ButtonRight;
        BRotate.Content = TXTS.ButtonRotate;
        LScore.Content = TXTS.LabelScore + score.ToString();
        MIGame.Header = TXTS.MIGame;
        MIHelp.Header = TXTS.MIHelp;
        LGameOver.Content = TXTS.LabelGameOver;
        MIEnd.Header = TXTS.MIEnd;
        MINewGame.Header = TXTS.MINewGame;
        MISettings.Header = TXTS.MISettings; 
        if (timer != null && timer.IsEnabled) { BStart.Content = TXTS.ButtonStart_pause; }
        else { BStart.Content = TXTS.ButtonStart; }
    }
    private void CBLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (BLeft != null) LanguageChange();
    }
    private void MINewGame_Click(object sender, RoutedEventArgs e) {
        if (timer != null && !timer.IsEnabled) {
            LGameOver.Visibility = Visibility.Collapsed;
            NewGame();
            currentShape = CreateNewShape();
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
            SetSettings(LoadSetting());
            SetUI();
            NewGame();
        }
        else {
        }      
    }

    private string[] LoadSetting() {
        StreamReader sr;
        try {
            sr = new StreamReader(settingFile);
        }
        catch (FileNotFoundException) {
            StreamWriter sw = new StreamWriter(settingFile);
            sw.WriteLine("20,10,20,5,");
            sw.Close();
            sr = new StreamReader(settingFile);
        }
        string[] strings = sr.ReadLine().Split(",");
        sr.Close();
        return strings;
    }
    private void SetSettings(string[] settings) {
        rows = int.Parse(settings[0]);
        columns = int.Parse(settings[1]);
        squareSize = int.Parse(settings[2]);
        speed = 550 - int.Parse(settings[3]) * 50; 
        if (timer != null) timer.Interval = TimeSpan.FromMilliseconds(speed);
    }

    private void MIHelp_Click(object sender, RoutedEventArgs e) {
        MessageBox.Show(WTetris, TXTS.MIHelpMessage);
    }
}