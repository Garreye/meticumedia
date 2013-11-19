using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Meticumedia.Helpers
{
    public class DefaultColorConverter : IValueConverter
    {
        Color IS_DEFAULT_COLOR = Colors.Blue;
        
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                return new SolidColorBrush((bool)value ? IS_DEFAULT_COLOR : Colors.Black);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is SolidColorBrush)
            {
                return ((SolidColorBrush)value).Color.Equals(IS_DEFAULT_COLOR);
            }
            return value;
        }

        #endregion
    }
}
