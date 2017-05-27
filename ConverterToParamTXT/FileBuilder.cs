using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConverterToParamTXT
{
    public abstract class FileBuilder
    {
        protected DirectoryInfo sParentPath;
        protected String sPath = String.Empty;
        protected List<ParamTXTFile> lstParamTXTFile = new List<ParamTXTFile>();
        protected ProcessedFile oFile;

        public DirectoryInfo ParentPath {
            get {
                return this.sParentPath;
            }
            set {
                this.sParentPath = value;
            }
        }
        public virtual void Initialize(String sPath)
        {
            this.sPath = sPath;
        }

        public virtual List<ParamTXTFile> Start()
        {
            return null;
        }

        public virtual void Finish()
        { 
        
        }

        public List<ParamTXTFile> Result()
        {
            return this.lstParamTXTFile;
        }
    }
}
