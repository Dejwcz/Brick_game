using System;
using System.Collections.Generic;
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

namespace Brick_game
{
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
