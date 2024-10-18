using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Assistant.Tools.Converters
{
    public class ProtocolToTargetVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is String protocol)
            {
                return protocol switch
                {
                    "UDP" => Visibility.Visible,
                    "TCP Client" => Visibility.Collapsed,
                    "TCP Server" => Visibility.Collapsed,
                    _ => throw new NotImplementedException()
                };
            }
            throw new NotImplementedException("ProtocolToTargetVisibility Error");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
