using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Xml.Serialization;

namespace Brick_game;

[Serializable]
public class Record : INotifyPropertyChanged {
    [XmlAttribute]
    private string _name;
    [XmlAttribute]
    private int _score;
    [XmlAttribute]
    private DateTime _time;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name { get => _name; set => _name = value; }
    public int Score { get => _score; set => _score = value; }
    public DateTime Time { get => _time; set => _time = value; }
    public Record(string name, int score) {
        _name = name;
        _score = score;
        _time = DateTime.Now;
    }

    public Record() { }

    protected void OnPropertyChanged([CallerMemberName] string name = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
[Serializable]
public class HighScores {
    [XmlAttribute]
    private ObservableCollection<Record> _records;
    public ObservableCollection<Record> Records { get => _records; set => _records = value; }

    public HighScores(ObservableCollection<Record> records) {
        _records = records;
    }

    public HighScores() {
        _records = new ObservableCollection<Record>();
    }

    public void AddRecord(Record record) {
        _records.Add(record);
    }

    public void RemoveRecord(Record record) { _records.Remove(record); }

    public void ClearAll() { _records.Clear(); }

    public ObservableCollection<Record> GetXFirstHighScores(int count) {
        return new ObservableCollection<Record>(
        this.Records
            .OrderByDescending(record => record.Score)
            .Take(count)
            );
    }

    public void SaveToFile(string filename) {
        string dataFile = (Directory.GetCurrentDirectory() + "\\Resources\\" + filename);
        FileStream fs = new FileStream(dataFile, FileMode.Create);
        XmlSerializer xs = new XmlSerializer(typeof(HighScores));
        xs.Serialize(fs, this);
        fs.Close();
    }

    public static HighScores LoadFromFile(string filename) {
        HighScores scores = new HighScores();
        string dataFile = (Directory.GetCurrentDirectory() + "\\Resources\\" + filename);
        try {
            using (FileStream fs = new FileStream(dataFile, FileMode.Open)) {
                XmlSerializer xs = new XmlSerializer(typeof(HighScores));
                scores = (HighScores)xs.Deserialize(fs);
            }
        }
        catch (FileNotFoundException) {; }
        catch (Exception e) { MessageBox.Show(e.Message); }
        return scores;
    }
}
