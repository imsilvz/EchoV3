using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Utility
{
    public class Helpers
    {
        public static string ToHex(byte[] buffer)
        {
            // here we perform black magic
            byte[] bCopy = buffer.ToArray();
            char[] c = new char[bCopy.Length * 2];
            int b;
            for (int i = 0; i < bCopy.Length; i++)
            {
                b = bCopy[i] >> 4;
                c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
                b = bCopy[i] & 0xF;
                c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
            }
            return new string(c);
        }
    }
}
