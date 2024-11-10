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
/// Class with settings for Brick game. Colors are saved as string because Brush cannot be serialized
/// </summary>
[Serializable]
public class AppSettings{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public int SquareSize { get; set; }
    public int Speed { get; set; }                                    //Speed of falling in "miliseconds"
    public string BackgroundColor { get; set; }                       //Background of gameboard color
    public string BrickColor { get; set; }                            //Brick color
    public string GridColor { get; set; }                             //Grid color
    public bool PlayOnStartApp { get; set; }

    /// <summary>
    /// Empty constructor for default settings
    /// </summary>
    public AppSettings() {
        Rows = 20;
        Columns = 10;
        SquareSize = 10;
        Speed = 5;
        BackgroundColor = "#FF0000FF";
        BrickColor = "#FFFF0000";
        GridColor = "#FF000000";
    }
    public AppSettings(int rows, int columns, int squareSize, int speed, string backgroundColor, string brickColor, string gridColor, bool playOnStartApp) {
        Rows = rows;
        Columns = columns;
        SquareSize = squareSize;
        Speed = speed;
        this.BackgroundColor = backgroundColor;
        this.BrickColor = brickColor;
        this.GridColor = gridColor;
        PlayOnStartApp = playOnStartApp;
    }

    public void SetRows(int rows) { this.Rows = rows; }
    public void SetColumns(int columns) { this.Columns = columns; }
    public void SetSpeed(int speed) { this.Speed = speed; }
    public void SetSquareSize(int squareSize) { this.SquareSize = squareSize; }
    public void SetBackgroundColor(Brush background) { this.BackgroundColor = background.ToString(); }
    public void SetBrickColor(Brush brickcolor) { this.BrickColor = brickcolor.ToString(); }
    public void SetGridColor(Brush gridcolor) { this.GridColor = gridcolor.ToString(); }
    public void SetPlayOnStartApp(bool playMusic) { this.PlayOnStartApp = playMusic; }
    public void SetAll(int rows, int columns, int squareSize, int speed, Brush backgroundColor, Brush brickColor, Brush gridColor, bool playMusic) {
        SetRows(rows);
        SetColumns(columns);
        SetSpeed(speed);
        SetSquareSize(squareSize);
        SetBackgroundColor(backgroundColor);
        SetBrickColor(brickColor);
        SetGridColor(gridColor);
        SetPlayOnStartApp(playMusic);
    }
    public Brush GetBackgroundColor() {
        return (Brush)new BrushConverter().ConvertFromString(BackgroundColor);
    }
    public Brush GetBrickColor() {
        return (Brush)new BrushConverter().ConvertFromString(BrickColor);
    }
    public Brush GetGridColor() {
        return (Brush)new BrushConverter().ConvertFromString(GridColor);
    }
    public void SaveToFile(string filename) {
        string dataFile = (Directory.GetCurrentDirectory() + "\\Resources\\" + filename);
        FileStream fs = new FileStream(dataFile, FileMode.Create);
        XmlSerializer xs = new XmlSerializer(typeof(AppSettings));
        xs.Serialize(fs, this);
        fs.Close();
    }
    public static AppSettings LoadFromFile(string filename) {
        AppSettings appSettings = new AppSettings();
        string dataFile = (Directory.GetCurrentDirectory() + "\\Resources\\" + filename);
        try {
            using (FileStream fs = new FileStream(dataFile, FileMode.Open)) {
                XmlSerializer xs = new XmlSerializer(typeof(AppSettings));
                appSettings = (AppSettings)xs.Deserialize(fs);
            }
        }
        catch (FileNotFoundException) {; }
        catch (Exception e) { MessageBox.Show(e.Message); }
        return appSettings;
    }
}