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

    private int score = 0;                              //Score
    private bool gameIsOn = false;                      //Game is running
    private bool isClearBoard = true;                   //Game board is cleared
    private bool oBrick;                                //If current shape is OBrick
    private Random rand = new Random();

    private Canvas[] squares;
    private Grid gameField;

    private int[] currentShape;
    private DispatcherTimer timer;
    private int countOfHighscores = 5;
    private Shapes shapes;
    private HighScores highScores;
    private string highScoresFile = "highscores.xml";
    private AppSettings appSettings;
    private string settingsFile = "settings.xml";

    public MainWindow() {

        //!!!Do not forget to update LanguageChange() and WTetris_Loaded() if there was added new UI element with text. Data binding does not working well in this window 

        InitializeComponent();
        appSettings = AppSettings.LoadFromFile(settingsFile);                                                                 

        highScores = HighScores.LoadFromFile(highScoresFile);                                       //Load high scores from file
        SetHighScores(highScores.GetXFirstHighScoresToString(countOfHighscores));                   //Set high scores to Labels

        shapes = new Shapes(appSettings.Columns);
        SetUI();                                                                                    //method to set UI acording to values in setting file.
    }
    private void SetUI() {

        #region gameField
        MainContainer.Children.Clear();
        gameField = new Grid {
            Width = appSettings.Columns * (appSettings.SquareSize + 2),
            Height = appSettings.Rows * (appSettings.SquareSize + 2),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top
        };

        for (int i = 0; i < appSettings.Rows; i++) { gameField.RowDefinitions.Add(new RowDefinition()); }
        for (int i = 0; i < appSettings.Columns; i++) { gameField.ColumnDefinitions.Add(new ColumnDefinition()); }
        squares = new Canvas[appSettings.Rows * appSettings.Columns];
        for (int i = 0; i < appSettings.Rows * appSettings.Columns; i++) {
            squares[i] = new Canvas { Height = appSettings.SquareSize, Width = appSettings.SquareSize, Background = appSettings.GetBackgroundColor() };

            int row = i / appSettings.Columns;                  //Row position determination
            int col = i % appSettings.Columns;                  //Column position determination

            Border border = new Border {
                BorderBrush = appSettings.GetGridColor(),   // Line color
                BorderThickness = new Thickness(1), // 
                Child = squares[i]                  // Adding square into borders
            };

            Grid.SetRow(border, row);               //
            Grid.SetColumn(border, col);            //Set row and column
            gameField.Children.Add(border);         //Adding square with borders into grid
        }
        #endregion
        #region WTetrisAndItemsPropertiesSettings
        MainContainer.Children.Add(gameField);
        MainContainer.Height = gameField.Height;
        MainContainer.Width = gameField.Width;

        WTetris.ResizeMode = ResizeMode.NoResize;
        WTetris.Height = gameField.Height + 120 < 480 ? 480 : gameField.Height + 120;
        WTetris.Width = gameField.Width + 310 < 450 ? 450 : gameField.Width + 310;
        LGameOver.Margin = new Thickness(35 + (gameField.Width - LGameOver.Width) / 2, (gameField.Height / 2), 0, 0);
        LGameOver.Visibility = Visibility.Collapsed;
        #endregion
  
        currentShape = CreateNewShape();
    }
    public void GameOn() {                                                      //Start game    
        if (timer == null) {
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(550 - appSettings.Speed * 50) };
            timer.Tick += (s, e) => MoveDown(currentShape);
        }
        timer.Start();
    }
    public void NewGame() {                                                     //Clears board
        {
            foreach (Canvas b in squares) { b.Background = appSettings.GetBackgroundColor(); }
        }
        isClearBoard = true;
        score = 0;
        LScore.Content = TXTS.LabelScore + score.ToString();
    }
    private void MoveDown(int[] shape) {
        isClearBoard = false; 
        bool canMove = true;

        List<int> bricksToCheck = new List<int>();                                                  //Finding bottom bricks, for test of contact it needs only the bottom bricks
        foreach (int b in shape) {                                                                    
            bool isBottomBrick = true;
            foreach (int other in shape) {
                if (other != b && other == b + appSettings.Columns) { isBottomBrick = false; break; }
            }
            if (isBottomBrick) { bricksToCheck.Add(b); }
        }
        foreach (int b in bricksToCheck) {                                                            //Checking free space to move
            if (b + appSettings.Columns >= appSettings.Rows * appSettings.Columns || squares[b + appSettings.Columns].Background.ToString() == appSettings.BrickColor) {
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
            foreach (int b in shape) {
                squares[b].Background = appSettings.GetBackgroundColor();
            }
            for (int i = 0; i < shape.Length; i++) {
                shape[i] += appSettings.Columns;
                squares[shape[i]].Background = appSettings.GetBrickColor();
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
        foreach (int b in currentShape) { squares[b].Background = appSettings.GetBrickColor(); }
        timer.Stop();
        gameIsOn = false;
        LGameOver.Visibility = Visibility.Visible;              //Shows game over
        if (score > 0) {                                        //If is any score saves it and actualizes top game scores
            EnterNewScore();
            SetHighScores(highScores.GetXFirstHighScoresToString(countOfHighscores));
            highScores.SaveToFile("highscores.xml");
        }
    }
    private void EnterNewScore() {                  //Opens window to enter new name and save record to highscores records
        WEnterName wEnterName = new WEnterName();
        wEnterName.Owner = this;
        wEnterName.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        wEnterName.ShowDialog();
        highScores.AddRecord(new Record(wEnterName.TBEnterName.Text, score));
        wEnterName.Close();
    }
    private void MoveRight(int[] shape) {
        bool canMove = true;

        List<int> bricksToCheck = new List<int>();                                                  //Finding right bricks, for test of contact it needs only the right bricks
        foreach (int b in shape) {                                                                    
            bool isLeftBrick = true;
            foreach (int other in shape) {
                if (other != b && other == b + 1) { isLeftBrick = false; break; }
            }
            if (isLeftBrick) { bricksToCheck.Add(b); }
        }
        foreach (int b in bricksToCheck) {                                                            //Checking free space to move (edge or another brick)
            if (b % appSettings.Columns == appSettings.Columns -1 || squares[b + 1].Background.ToString() == appSettings.BrickColor) {
                canMove = false;
                break;
            }
        }
        if (canMove) {                                                                                  //Moving
            foreach (int b in shape) {
                squares[b].Background = appSettings.GetBackgroundColor();
            }
            for (int i = 0; i < shape.Length; i++) {
                shape[i] += 1;
                squares[shape[i]].Background = appSettings.GetBrickColor();
            }
        }
    }
    private void MoveLeft(int[] shape) {
        bool canMove = true;

        List<int> bricksToCheck = new List<int>();                                                  //Finding left bricks, for test of contact it needs only the left bricks
        foreach (int b in shape) {                                                                    
            bool isLeftBrick = true;
            foreach (int other in shape) {
                if (other != b && other == b - 1) { isLeftBrick = false; break; }
            }
            if (isLeftBrick) { bricksToCheck.Add(b); }
        }
        foreach (int b in bricksToCheck) {                                                            //Checking free space to move (edge or another brick)
            if (b % appSettings.Columns == 0 || squares[b - 1].Background.ToString() == appSettings.BrickColor) {
                canMove = false;
                break;
            }
        }
        if (canMove) {                                                                                  //Moving
            foreach (int b in shape) {
                squares[b].Background = appSettings.GetBackgroundColor();
            }
            for (int i = 0; i < shape.Length; i++) {
                shape[i] -= 1;
                squares[shape[i]].Background = appSettings.GetBrickColor();
            }
        }
    }
    private void RotateShape(int[] shape) {
        if (!oBrick) {                                                           //If shape is OBrick no rotation needed
            int pivot = shape[1];                                              //Second brick is selected like pivot for turning
            int pivotx = pivot % appSettings.Columns;
            int pivoty = pivot / appSettings.Columns;
            lock (this) {
                foreach (int b in shape) {
                    if (b != pivot) {
                        int relativeX = b % appSettings.Columns - pivotx;                //Getting relative coordinates
                        int relativeY = b / appSettings.Columns - pivoty;
                        int rotatedX = -relativeY;                               //Rotation
                        int rotatedY = relativeX;
                        int newX = pivotx + rotatedX;                            //Getting absolute coordinates
                        int newY = pivoty + rotatedY;
                        int newPos = newY * 10 + newX;
                        if (newX < 0 || newX >= appSettings.Columns || newY < 0 || newY >= appSettings.Rows ||
                        (squares[newPos].Background.ToString() == appSettings.BrickColor && !shape.Any(brick => brick == newPos))) return;
                    }
                }
                int[] newShape = new int[shape.Length];

                for (int i = 0; i < shape.Length; i++) {
                    int original = shape[i];
                    newShape[i] = original;
                }
                for (int i = 0; i < newShape.Length;i++) {
                    if (newShape[i] != pivot) {                    
                        int relativeX = newShape[i] % appSettings.Columns - pivotx;
                        int relativeY = newShape[i] / appSettings.Columns - pivoty;
                        newShape[i] = (pivoty + relativeX) * appSettings.Columns + (pivotx + (-relativeY));
                    }
                }
                for (int i = 0; i < shape.Length; i++) { squares[shape[i]].Background = appSettings.GetBackgroundColor(); }
                if (CanPlaceNewShape(newShape)) {
                    for (int j = 0; j < shape.Length; j++) {
                        if (shape[j] != pivot) {
                            int relativeX = shape[j] % appSettings.Columns - pivotx;
                            int relativeY = shape[j] / appSettings.Columns - pivoty;
                            shape[j] = (pivoty + relativeX) * appSettings.Columns + (pivotx + (-relativeY));
                        }
                        squares[shape[j]].Background = appSettings.GetBrickColor();
                    }
                }
                else { foreach (int b in shape) { squares[b].Background = appSettings.GetBrickColor(); } }
            }
        }
    }
    private bool CanPlaceNewShape(int[] shape) {                                                  //Checking space for new shape
        foreach (int b in shape) {
            if (squares[b].Background.ToString() == appSettings.BrickColor) {
                return false;
            }
        }
        return true;
    }
    private bool CompareShapes(int[] shape1, int[] shape2) {
        if (shape1.Length != shape2.Length) {return false; }
        for (int i = 0; i < shape1.Length; i++) {
            if (!(shape1[i] == shape2[i])) { return false; }
        }
        return true;
    }
    private int[] CreateNewShape() {                                                              //Making deep copy from array of shapes for moving with copy and leaving the original unchanged
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
        for (int i = 0; i < appSettings.Rows; i++) {
        fullRow = true;
            for (int j = 0 + appSettings.Columns * i; j < appSettings.Columns + appSettings.Columns * i; j++) {
                if (squares[j].Background.ToString() == appSettings.BackgroundColor) {                              //If finds empty field skips row
                    fullRow = false;
                    break;
                }
            }
            if (fullRow) {
                rowCleared++;
                for (int k = 0 + appSettings.Columns * i; k < appSettings.Columns + appSettings.Columns * i; k++) { squares[k].Background = appSettings.GetBackgroundColor(); }
                if (i == appSettings.Rows - 1) {
                    for (int l = (appSettings.Columns * appSettings.Rows) - 1; l >= appSettings.Columns; l--) { squares[l].Background = squares[l - appSettings.Columns].Background; }
                }
                else {
                    for (int l = (appSettings.Columns + appSettings.Columns * i) - 1; l >= appSettings.Columns; l--) { squares[l].Background = squares[l - appSettings.Columns].Background; }
                }
            }
        }
        return rowCleared;
    }
    /// <summary>
    /// Set high scores values to corresponding labels
    /// </summary>
    /// <param name="highScores"></param>
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
        LScore.Content = TXTS.LabelScore + score.ToString();
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
        if (timer != null && timer.IsEnabled) { BStart.Content = TXTS.ButtonStart_pause; }
        else { BStart.Content = TXTS.ButtonStart;}

    }
    private void WTetris_Loaded(object sender, RoutedEventArgs e) {                 // IN main window data binding does not working good
        BLeft.Content = TXTS.ButtonLeft;
        BRight.Content = TXTS.ButtonRight;
        BRotate.Content = TXTS.ButtonRotate;
        LScore.Content = TXTS.LabelScore + score.ToString();
        MIGame.Header = TXTS.MIGame;
        MIHelp.Header = TXTS.MIHelp;
        LGameOver.Content = TXTS.LabelGameOver;
        MIEnd.Header = TXTS.MIEnd;
        MIMusic.Header = TXTS.MIMusic;
        MINewGame.Header = TXTS.MINewGame;
        MISettings.Header = TXTS.MISettings;
        MIHighScores.Header = TXTS.MIHighScores;
        if (timer != null && timer.IsEnabled) { BStart.Content = TXTS.ButtonStart_pause; }
        else { BStart.Content = TXTS.ButtonStart; }
        if (appSettings.PlayOnStartApp && MIMusic.IsChecked) { MEMusic.Play(); }
        else { MIMusic.IsChecked = false; }
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
        if (e.Key == Key.N) {
            if (timer != null && !timer.IsEnabled) {
                LGameOver.Visibility = Visibility.Collapsed;
                NewGame();
                currentShape = CreateNewShape();
                BStart.Content = TXTS.ButtonStart;
            }
        }
        if (e.Key == Key.Down) { if (timer != null && timer.IsEnabled) MoveDown(currentShape); }
        //if (e.Key == Key.T) { MessageBox.Show(Directory.GetCurrentDirectory()); }  //test key
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
    private void BRotate_Click(object sender, RoutedEventArgs e) {
        if (timer != null && timer.IsEnabled) RotateShape(currentShape);
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
            appSettings = AppSettings.LoadFromFile(settingsFile);
            if (timer != null) timer.Interval = TimeSpan.FromMilliseconds(550 - appSettings.Speed * 50); // Higher speed means run faster
            SetUI();
            NewGame();
            if (timer != null && timer.IsEnabled) { BStart.Content = TXTS.ButtonStart_pause; }
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
        appSettings.SaveToFile(settingsFile);
    }
    private void MIHighScores_Click(object sender, RoutedEventArgs e) {
        WHighScores wHighScores = new WHighScores();
        wHighScores.Owner = this;
        wHighScores.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        wHighScores.ShowDialog();
    }
}