using System;
using System.Collections.Generic;
using System.IO;
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
using TXTS = Brick_game.Properites.Resources;


namespace BrickGame {
    /// <summary>
    /// Interaction logic for WSettings.xaml
    /// </summary>
    public partial class WSettings : Window {
        private string settingFile = Directory.GetCurrentDirectory() + "\\text.txt";
        public WSettings() {
            InitializeComponent();
            SetValues(LoadSetting());   

            #region InicializeTexts
            this.Title = TXTS.WSettingsTitle;
            GBSquareSize.Header = TXTS.GBSquareSize;
            GBStartSpeed.Header = TXTS.GBStartSpeed;
            GBGameboardSize.Header = TXTS.GBGameboardSize;
            LHeigth.Content = TXTS.LHeight;
            LWidth.Content = TXTS.LWidth;
            LSquareSize.Content = TXTS.LSquareSize;
            LStartSpeed.Content = TXTS.LStartSpeed;
            BBack.Content = TXTS.BBack;
            BSave.Content = TXTS.BSave;
            BDefault.Content = TXTS.BDefault;
            #endregion
        }
        private void SaveSettings() {
            TestAll();
            string settingsValues = "";
            settingsValues += TBHeigth.Text + ",";
            settingsValues += TBWidth.Text + ",";
            settingsValues += TBSquareSize.Text + ",";
            settingsValues += TBStartSpeed.Text + ",";

            StreamWriter streamWriter = new StreamWriter(settingFile);
            streamWriter.Write(settingsValues);
            streamWriter.Close();
        }
        private string[] LoadSetting() {
            StreamReader sr = new StreamReader(settingFile);           
            string[] strings = sr.ReadLine().Split(",");
            sr.Close();
            return strings;
        }
        /// <summary>
        /// Set values for gameboard
        /// </summary>
        /// <param name="values"></param>
        private void SetValues(string[] values) {
            TBHeigth.Text = values[0];
            TBWidth.Text = values[1];
            TBSquareSize.Text = values[2];
            TBStartSpeed.Text = Convert.ToString(int.Parse(values[3]));
        }
        /// <summary>
        /// Without args set default values for gameboard
        /// </summary>
        private void SetValues() {
            TBHeigth.Text = "20";
            TBWidth.Text = "10";
            TBSquareSize.Text = "20";
            TBStartSpeed.Text = "5";
        }
        private void TestAll() {
            TestHeigth();
            TestWidth();
            TestSquareSize();
            TestStartSpeed();
        }
        private void TestHeigth() {
            if (!int.TryParse(TBHeigth.Text, out int num)) {
                TBHeigth.Text = "20";
            }
            else {
                if (num >= 100) { TBHeigth.Text = "100"; };
                if (num <= 10) { TBHeigth.Text = "10"; }
            }
        }
        private void TestWidth() {
            if (!int.TryParse(TBWidth.Text, out int num)) {
                TBWidth.Text = "10";
            }
            else {
                if (num >= 100) { TBWidth.Text = "100"; };
                if (num <= 10) { TBWidth.Text = "10"; }
            }
        }
        private void TestSquareSize() {
            if (!int.TryParse(TBSquareSize.Text, out int num)) {
                TBSquareSize.Text = "20";
            }
            else {
                if (num >= 40) { TBSquareSize.Text = "40"; };
                if (num <= 2) { TBSquareSize.Text = "2"; }
            }
        }
        private void TestStartSpeed() {
            if (!int.TryParse(TBStartSpeed.Text, out int num)) {
                TBStartSpeed.Text = "5";
            }
            else {
                if (num >= 10) { TBStartSpeed.Text = "10"; };
                if (num <= 1) { TBStartSpeed.Text = "1"; }
            }
        }
        private void BBack_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
            this.Close();
        }
        private void BSave_Click(object sender, RoutedEventArgs e) {
            SaveSettings();
            this.DialogResult=true;
            this.Close();
        }
        private void TBHeigth_LostFocus(object sender, RoutedEventArgs e) {
            TestHeigth();
        }
        private void TBWidth_LostFocus(object sender, RoutedEventArgs e) {
            TestWidth();
        }
        private void TBSquareSize_LostFocus(object sender, RoutedEventArgs e) {
            TestSquareSize();
        }
        private void TBStartSpeed_LostFocus(object sender, RoutedEventArgs e) {
            TestStartSpeed();
        }
        private void BDefault_Click(object sender, RoutedEventArgs e) {
            SetValues();
        }     
    }
}
