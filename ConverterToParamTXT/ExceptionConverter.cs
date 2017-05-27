using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ConverterToParamTXT
{
    [Serializable]
    public class ExceptionConverter: ApplicationException
    {
        public ExceptionConverter() { }
        public ExceptionConverter(string message): base(message){}
        protected ExceptionConverter(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public ExceptionConverter(string message, Exception innerException) : base(message, innerException) { }
    }
}
