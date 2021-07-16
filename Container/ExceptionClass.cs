using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container
{
    class ExceptionClass:ApplicationException
    {

    }

    [Serializable]
    public class FileSearchException : Exception
    {
        public FileSearchException() { }
        public FileSearchException(string message) : base(message) { }
        public FileSearchException(string message, Exception inner) : base(message, inner) { }
        protected FileSearchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
