using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ConverterToParamTXT
{
    [Serializable]
    class MonitorAdapterException: ApplicationException
    {
        public MonitorAdapterException() { }
        public MonitorAdapterException(string message) : base(message) { }
        protected MonitorAdapterException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public MonitorAdapterException(string message, Exception innerException) : base(message, innerException) { }
        
    }
}
