using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Brick_game.Converters;
public class ButtonContentConverter : IMultiValueConverter {

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {

        if (values.Length < 2 || !(values[0] is bool gameIsOn) || !(values[1] is bool isGameActive))
            return Application.Current.Resources["ButtonNewGame"]; // Výchozí hodnota

        if (!gameIsOn)
            return Application.Current.Resources["ButtonNewGame"];
        return isGameActive ? Application.Current.Resources["ButtonPause"] : Application.Current.Resources["ButtonStart"];
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
