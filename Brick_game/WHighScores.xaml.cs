using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Brick_game {
    /// <summary>
    /// Interaction logic for WHighScores.xaml
    /// </summary>
    public partial class WHighScores : Window {
        public WHighScores() {
            InitializeComponent();

            string headerTextName = (string)Application.Current.Resources["Name"];
            string headerTextScore = (string)Application.Current.Resources["Score"];
            string headerTextTime = (string)Application.Current.Resources["Time"];

            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            HighScores highScores = new HighScores();
            highScores = HighScores.LoadFromFile("highscores.xml");
            DGHighScores.AutoGenerateColumns = false;

            DataGridTextColumn nameColumn = new DataGridTextColumn {                                    // Column for "Name"
                Header = headerTextName,
                IsReadOnly = true,
                Binding = new Binding("Name")
            };
            DataGridTextColumn scoreColumn = new DataGridTextColumn {                                   // Column for "Score"
                Header = headerTextScore,
                IsReadOnly = true,
                Binding = new Binding("Score")
            };
            DataGridTextColumn timeColumn = new DataGridTextColumn {                                    // Column for "Time" with format
                Header = headerTextTime,
                IsReadOnly = true,
                Binding = new Binding("Time") {
                StringFormat = currentCulture.DateTimeFormat.ShortDatePattern + " " + currentCulture.DateTimeFormat.LongTimePattern,
                }                                                                                       //Shows date and time acording to actual language
            };
            DGHighScores.Columns.Add(nameColumn);                                                       //Adding Columns to datagrid
            DGHighScores.Columns.Add(scoreColumn);
            DGHighScores.Columns.Add(timeColumn);

            DGHighScores.ItemsSource = highScores.GetXFirstHighScores(highScores.Records.Count());      //Sorted source of data
        }

        private void BBack_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
