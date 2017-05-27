using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConverterToParamTXT
{
    public class InvoiceCls: InvoiceAbstract
    {
        public InvoiceCls(List<DataInvoice> lstData)
        {
            this.lstData = lstData;
        }
    }
}
