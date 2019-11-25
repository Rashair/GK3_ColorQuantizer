using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK3_ColorQuantizer.Algorithms
{
    static class Helpers
    {
        public static byte ToByte(this int i)
        {
            return (byte)(i > 255 ? 255 : i);
        }
    }
}
