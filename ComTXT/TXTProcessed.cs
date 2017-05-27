using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConverterToParamTXT;
using System.IO;

namespace ComTXT
{
    public class TXTProcessed: ProcessedFile
    {
        private String strRead = String.Empty;
        public TXTProcessed(String sPath)
            : base(sPath)
        {

        }
        public String StrRead
        {
            get
            {
                return this.strRead;
            }
        }
        public override void ReadFile()
        {
            using (FileStream fs = new FileStream(this.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                {
                    this.strRead = sr.ReadToEnd();
                }
            }
        }

        public override void ProcessedResult()
        {

        }
    }
}
