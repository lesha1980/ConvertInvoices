using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data;

namespace ConverterToParamTXT
{
    public abstract class BarCode : DataInvoice
    {
        public String sNameCode { get; set; }   
        public BarCode()
        {
            this.sNameData = "Штрихкод";
          
        }

    }
}
