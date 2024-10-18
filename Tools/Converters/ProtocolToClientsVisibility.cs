using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Assistant.Tools.Converters
{
    public class ProtocolToClientsVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is String protocol)
            {
                return protocol switch
                {
                    "UDP" => Visibility.Hidden,
                    "TCP Client" => Visibility.Hidden,
                    "TCP Server" => Visibility.Visible,
                    _ => throw new NotImplementedException()
                };
            }
            throw new NotImplementedException("ProtocolToVisibility Error");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
