using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConverterToParamTXT
{
    public class FileConverter
    {
        private ProcessedFile oFile;
        private FileBuilder oFileBuilder;

        public FileConverter()
        { 
        
        }

        public FileConverter(ProcessedFile oFile, FileBuilder oFileBuilder)
        {
            this.oFile = oFile;
            this.oFileBuilder = oFileBuilder;
        }
        public object Convert()
        {
           // this.oFileBuilder.Initialize();
            this.oFileBuilder.Start();
            this.oFileBuilder.Finish();
            return this.oFileBuilder.Result();
        }
    }
}
