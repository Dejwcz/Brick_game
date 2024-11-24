using System.Windows.Data;
using System.Globalization;

namespace Brick_game.Converters;
    public class BoolToColorConverter() : IValueConverter {
        private AppSettings _appSettings;
       
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            _appSettings = AppSettings.LoadFromFile("settings.xml");
            bool isOn = (bool)value;
            return isOn ? _appSettings.BrickColorBrush : _appSettings.BackgroundColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }