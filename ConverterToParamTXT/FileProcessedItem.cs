using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ConverterToParamTXT
{
    [Serializable]
    public class FileProcessedItem
    {
        public String DateProcessed { get; set; }
        public String NameFile { get; set; }
        public Boolean Status { get; set; }
    }
}
