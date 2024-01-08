using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Assistant.Tools.Helpers
{
    internal class SerialPortHelper
    {
        internal static String[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }
    }
}
