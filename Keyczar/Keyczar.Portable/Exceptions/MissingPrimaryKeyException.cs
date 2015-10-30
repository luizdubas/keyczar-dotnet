using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyczar.Portable.Exceptions
{
    public class MissingPrimaryKeyException : Exception
    {
        public MissingPrimaryKeyException()
        {
        }

        public MissingPrimaryKeyException(string message) : base(message)
        {
        }

        public MissingPrimaryKeyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
