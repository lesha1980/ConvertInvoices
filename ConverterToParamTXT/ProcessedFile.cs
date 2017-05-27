using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace ConverterToParamTXT
{
    public abstract class ProcessedFile
    {
        protected String sFileName;
        protected IDbConnection ExcelConnection;
        protected IDbCommand ExcelCommand;
        protected DbConnectionStringBuilder ExcelBuilder;
        protected IDbDataAdapter oDataAdapter;

        public ProcessedFile(String sFileName)
        {
            this.sFileName = sFileName;
        }
        public abstract void ReadFile();
        public abstract void ProcessedResult();
        public String FileName
        {
            get {
                return this.sFileName;
            }

            set {
                this.sFileName = value;
            }

        }

        ~ProcessedFile()
        {
            if (ExcelConnection != null)
                ExcelConnection.Dispose();
            if (ExcelCommand != null)
                ExcelCommand.Dispose();
            
          
        }

    }
}
