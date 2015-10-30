using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyczar.Portable.Extensions
{
    public static class EncodingExtensions
    {
        public static string GetString(this Encoding encoding, byte[] bytes)
            =>
                encoding.GetString(bytes, 0, bytes.Length);
    }
}
