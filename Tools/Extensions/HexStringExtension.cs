using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assistant.Tools.Extensions
{
    public class HexStringExtension
    {
        public static Byte[] ToBytes(String hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += "0";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        public static Boolean IsValid(String hexString)
        {
            Byte[] pendingChars = Encoding.UTF8.GetBytes(hexString);
            for (Int32 i = 0; i < pendingChars.Length; i++)
            {
                if ((pendingChars[i] >= '0' && pendingChars[i] <= '9') ||
                    (pendingChars[i] >= 'A' && pendingChars[i] <= 'F') ||
                    (pendingChars[i] >= 'a' && pendingChars[i] <= 'f') ||
                    (pendingChars[i] == ' '))
                {

                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
