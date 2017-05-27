using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ConverterToParamTXT
{
    [Serializable]
    public class ParamTXTException: ExceptionConverter
    {
        public ParamTXTException() { }
        public ParamTXTException(string message): base(message){}
        protected ParamTXTException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public ParamTXTException(string message, Exception innerException) : base(message, innerException) { }

    }
}
