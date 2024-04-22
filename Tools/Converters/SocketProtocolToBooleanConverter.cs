using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Assistant.Tools.Converters
{
    public class SocketProtocolToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is String protocol)
            {
                return protocol switch {
                    "UDP Client" => true,
                    "UDP Server" => false,
                    "TCP Client" => true,
                    "TCP Server" => false,
                    _ => throw new NotImplementedException()
                };
            }
            throw new NotImplementedException("SocketProtocolToBooleanConverter Error");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
