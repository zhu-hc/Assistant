using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assistant.Data.Models
{
    public class SerialPortConfig
    {
        public String PortName { get; set; }
        public Int32 BaudRate { get; set; }
        public Int32 DataBits { get; set; }
        public StopBits StopBits { get; set; }
        public Parity Parity { get; set; }
    }
}
