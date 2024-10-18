using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Assistant.Tools.Extensions
{
    public class EndPointExtension
    {
        public static EndPoint ToEndPoint(string endpointString)
        {
            // 分割字符串
            var parts = endpointString.Split(':');
            if (parts.Length != 2)
            {
                throw new ArgumentException("字符串格式不正确，应该为 'IP:Port'");
            }

            // 解析 IP 地址和端口
            var ipAddress = IPAddress.Parse(parts[0]);
            var port = int.Parse(parts[1]);

            // 创建 IPEndPoint
            return new IPEndPoint(ipAddress, port);
        }
    }
}
