using Brick_game;
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
using TXTS = Brick_game.Properties.Resources;


namespace BrickGame {
    /// <summary>
    /// Interaction logic for WSettings.xaml
    /// </summary>
    public partial class WSettings : Window {

        private string settingFile = "settings.xml";
        private AppSettings appSettings;
        private AppSettingsViewModel viewModel;

        public WSettings() {
            InitializeComponent();
            appSettings = AppSettings.LoadFromFile(settingFile);
            viewModel = new AppSettingsViewModel(appSettings);
            this.DataContext = viewModel;

            SetColors();                   
            SetSelectedItemInComboBoxes();
        }
        private void SetColors() {
                LGBColor.Background = appSettings.GetBackgroundColor();
                LBrickColor.Background = appSettings.GetBrickColor();
                LGridColor.Background = appSettings.GetGridColor();
                ChangeTextColor(LGBColor, appSettings.GetBackgroundColor());                    
                ChangeTextColor(LBrickColor, appSettings.GetBrickColor());
                ChangeTextColor(LGridColor, appSettings.GetGridColor());
        }
        private void SetSelectedColorInComboBox(ComboBox comboBox, Brush color) {                      //for one ComboBox
            foreach (ComboBoxItem item in comboBox.Items) {
                if (item.Background is SolidColorBrush itemBrush && color is SolidColorBrush selectedBrush) {
                    if (itemBrush.Color.Equals(selectedBrush.Color)) {
                        comboBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }
        private void SetSelectedItemInComboBoxes() {                                                    //for all Comboboxes          
            SetSelectedColorInComboBox(CBGBColor, appSettings.GetBackgroundColor());
            SetSelectedColorInComboBox(CBGridColor, appSettings.GetGridColor());
            SetSelectedColorInComboBox(CBBrickColor, appSettings.GetBrickColor());
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
        private bool TestCBBrickColors() {
            if (CBBrickColor.SelectedIndex == -1) {
                MessageBoxResult result = MessageBox.Show(TXTS.MBMissingColor, TXTS.MBMissingColorHead, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes) {
                    appSettings.SetBackgroundColor(Brushes.Blue);
                    appSettings.SetBrickColor(Brushes.Red);
                    appSettings.SetGridColor(Brushes.Black);
                    return true;
                }
                return false;
            }
            return true;
        }
        private bool TestCBGBColors() {
            if (CBGBColor.SelectedIndex == -1) {
                MessageBoxResult result = MessageBox.Show(TXTS.MBMissingColor, TXTS.MBMissingColorHead, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes) {
                    appSettings.SetBackgroundColor(Brushes.Blue);
                    appSettings.SetBrickColor(Brushes.Red);
                    appSettings.SetGridColor(Brushes.Black);
                    return true;
                }
                return false;
            }
            return true;
        }
        /// <summary>
        /// Changes the label text color to white due to the dark background(At very first time change text color does not working, because Brush is not initialized at right time)
        /// </summary>
        /// <param name="color"></param>
        /// <param name="label"></param>
        private void ChangeTextColorOld(Label label, Brush color) {                                                                     
            List<Brush> darkBrushes = new List<Brush> { Brushes.Black, Brushes.Blue, Brushes.Gray, Brushes.Green, Brushes.Purple };
            if (darkBrushes.Contains(color)) {
                label.Foreground = Brushes.White;
            }
            else { label.Foreground = Brushes.Black; }
        }
        /// <summary>
        /// Changes the label text color to white due to the dark background
        /// </summary>
        /// <param name="color"></param>
        /// <param name="label"></param>
        private void ChangeTextColor(Label label, Brush color) {
            List<Color> darkColors = new List<Color> { Colors.Black, Colors.Blue, Colors.Gray, Colors.Green, Colors.Purple };

            var brushColor = (color as SolidColorBrush)?.Color;

            if (brushColor.HasValue && darkColors.Contains(brushColor.Value)) {
                label.Foreground = Brushes.White;
            }
            else {
                label.Foreground = Brushes.Black;
            }
        }
        private void BBack_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
            this.Close();
        }
        private void BSave_Click(object sender, RoutedEventArgs e) {
            if (TestCBBrickColors() && TestCBGBColors()) {
                TestAll();
                appSettings.SaveToFile(settingFile);
                this.DialogResult = true;
                this.Close();
            }
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
            SetColors();
            SetSelectedItemInComboBoxes();
        }

        private bool _isUpdatingComboBox = false;
        private void CBGBColor_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (CBGBColor.SelectedItem is ComboBoxItem selectedItem) {
                appSettings.SetBackgroundColor((Brush)new BrushConverter().ConvertFromString(selectedItem.Tag.ToString()));
                if (appSettings.GetBrickColor() is SolidColorBrush brickBrush && appSettings.GetBackgroundColor() is SolidColorBrush gbBrush) {
                    if (brickBrush.Color.Equals(gbBrush.Color)) {
                        MessageBox.Show(TXTS.MBSameColors);
                        _isUpdatingComboBox = true;
                        CBGBColor.SelectedIndex = -1;
                        LGBColor.Background = null;
                        LGBColor.Foreground = Brushes.Black;
                        _isUpdatingComboBox = false;
                    }
                    else {
                        LGBColor.Background = appSettings.GetBackgroundColor();
                        ChangeTextColor(LGBColor, appSettings.GetBackgroundColor());
                    }
                }
            }
        }
        private void CBBrickColor_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (_isUpdatingComboBox) return;
            if (CBBrickColor.SelectedItem is ComboBoxItem selectedItem) {
                appSettings.SetBrickColor((Brush)new BrushConverter().ConvertFromString(selectedItem.Tag.ToString()));
                if (appSettings.GetBrickColor() is SolidColorBrush brickBrush && appSettings.GetBackgroundColor() is SolidColorBrush gbBrush) {
                    if (brickBrush.Color.Equals(gbBrush.Color)) {
                        MessageBox.Show(TXTS.MBSameColors);
                        _isUpdatingComboBox = true;
                        CBBrickColor.SelectedIndex = -1;
                        LBrickColor.Background = null;
                        LBrickColor.Foreground = Brushes.Black;
                        _isUpdatingComboBox = false;
                    }
                    else {
                        LBrickColor.Background = appSettings.GetBrickColor();
                        ChangeTextColor(LBrickColor, appSettings.GetBrickColor());
                    }
                }
            }
        }
        private void CBGridColor_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (CBGridColor.SelectedItem is ComboBoxItem selectedItem) {
                appSettings.SetGridColor((Brush)new BrushConverter().ConvertFromString(selectedItem.Tag.ToString()));
                LGridColor.Background = appSettings.GetGridColor();
                ChangeTextColor(LGridColor, appSettings.GetGridColor());
            }
        }

    }
}
