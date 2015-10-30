using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyczar.Portable.Exceptions
{
    public class ReadOnlyException : Exception
    {
        public ReadOnlyException(string message) : base(message)
        {
        }
    }
}
