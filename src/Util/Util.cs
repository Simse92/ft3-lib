using System;
using System.Linq;
using System.Text;

namespace Chromia.Postchain.Ft3
{
    public static class Util
    {
        /**
        * Converts hex string to Buffer
        * @param key: string
        * @returns {Buffer}
        */
        public static byte[] HexStringToBuffer(string text)
        {
            return Enumerable.Range(0, text.Length)
                            .Where(x => x % 2 == 0)
                            .Select(x => Convert.ToByte(text.Substring(x, 2), 16))
                            .ToArray();
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        public static string AuthTypeToString(AuthType type)
        {
            switch(type)
            {
                case AuthType.SingleSig:
                    return "S";
                case AuthType.MultiSig:
                    return "M";
                default:
                    return "";
            }
        }
        
    }
}
