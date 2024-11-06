using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TXTS = Brick_game.Properties.Resources;

namespace Brick_game {
    /// <summary>
    /// Interaction logic for WHighScores.xaml
    /// </summary>
    public partial class WHighScores : Window {
        public WHighScores() {
            InitializeComponent();

            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            HighScores highScores = new HighScores();
            highScores = HighScores.LoadFromFile("highscores.xml");
            DGHighScores.AutoGenerateColumns = false;

            DataGridTextColumn nameColumn = new DataGridTextColumn {                                    // Column for "Name"
                Header = TXTS.Name,
                IsReadOnly = true,
                Binding = new Binding("Name")
            };
            DataGridTextColumn scoreColumn = new DataGridTextColumn {                                   // Column for "Score"
                Header = TXTS.Score,
                IsReadOnly = true,
                Binding = new Binding("Score")
            };
            DataGridTextColumn timeColumn = new DataGridTextColumn {                                    // Column for "Time" with format
                Header = TXTS.Time,
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
