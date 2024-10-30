using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Brick_game;
//
//1. If you want to serialize class with xml serializer you must have public properties or fields. So one of them is useless. And if you have both(fields and properties) is there a problem. You will have data twice in xml file.
//
//2. If you use [Serializable] and [XmlAttribute] to fields it seems to work 
//
[Serializable]
public class Record {
    [XmlAttribute]
    private string name;
    [XmlAttribute]
    private int score;
    [XmlAttribute]
    private DateTime time;
    public string Name { get => name; set => name = value; }
    public int Score { get => score; set => score = value; }
    public DateTime Time { get => time; set => time = value; }
    public Record(string name, int score) {
        this.Name = name;
        this.Score = score;
        this.Time = DateTime.Now;
    }
    public Record() { }
}
[Serializable]
public class HighScores {
    [XmlAttribute]
    private List<Record> records;
    public List<Record> Records { get => records; set => records = value; }
    public HighScores(List<Record> records) {
        this.records = records;
    }
    public HighScores() {
        this.records = new List<Record>();
    }
    public void AddRecord(Record record) {
            this.records.Add(record);          
    }
    public void RemoveRecord(Record record) { this.records.Remove(record); }
    public void ClearAll() { this.records.Clear(); }
    /// <summary>
    /// Gets array with highest records sorted from biggest
    /// </summary>
    /// <param name="count">How many records you want, if is bigger than actual number of records, returns all available records</param>
    /// <returns></returns>
    public Record[] GetXFirstHighScores(int count) {
        int realCount = count < this.Records.Count ? count : this.Records.Count;
        Record[] topScores = new Record[realCount];
        int max = int.MinValue;
        int tempMax = int.MaxValue;

        Record topRecord = null;
        for (int i = 0; i < count; i++) {
            foreach (Record record in Records) {
                if (record.Score <= tempMax && record.Score > max && !topScores.Contains(record)) {
                    max = record.Score;
                    topRecord = record;
                }
            }
            tempMax = max;
            max = int.MinValue;
            if (topRecord != null && !topScores.Contains(topRecord)) { topScores[i] = topRecord; }
            else { break; }
        }
        return topScores;
    }
    public string[] GetXFirstHighScoresToString(int count) {
        Record[] topScores = GetXFirstHighScores(count);
        string[] strings = new string[topScores.Length];
        for (int i = 0; i < topScores.Length; i++) {
            strings[i] = $"{i+1}. {topScores[i].Name.Substring(0, topScores[i].Name.Length > 10 ? 10 : topScores[i].Name.Length)}  {topScores[i].Score}";
        }
        return strings;
    }
    public void SaveToFile(string filename) {
        string dataFile = (Directory.GetCurrentDirectory() + "\\" + filename);
        FileStream fs = new FileStream(dataFile, FileMode.Create);
        XmlSerializer xs = new XmlSerializer(typeof(HighScores));
        xs.Serialize(fs, this);
        fs.Close();
    }
    public static HighScores LoadFromFile(string filename) {
        HighScores scores = new HighScores();
        string dataFile = (Directory.GetCurrentDirectory() + "\\" + filename);
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

