using System.Windows;
using System.Windows.Input;

namespace Brick_game {
    /// <summary>
    /// Interaction logic for WEnterName.xaml
    /// </summary>
    public partial class WEnterName : Window
    {
        public WEnterName()
        {
            InitializeComponent();
            TBEnterName.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            this.Hide();
        }

        private void TBEnterName_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) { Button_Click(sender, e); }
        }
    }
}
