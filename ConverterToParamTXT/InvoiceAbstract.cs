using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConverterToParamTXT
{
    public abstract class InvoiceAbstract
    {
        protected List<DataInvoice> lstData;
        protected String sInvoiceName = String.Empty;
       // protected int nCountSigns;

        public InvoiceAbstract()
        {
            this.lstData = new List<DataInvoice>();
            
        }
        
        public int CountDI {
            get {
                return this.lstData.Count;

            }
           
        }
        public DataInvoice this[int index]
        {
            get {
                return this.lstData[index];
            }
            set { 
                this.lstData[index] = value;
            }
        }

        public void SetData(BarCode oBarCode, ProductName oProduct, AmountTitle oAmount, PriceTitle oPrice1, PriceTitle oPrice2)
        {
            this.lstData.Add(oBarCode);
            this.lstData.Add(oProduct);
            this.lstData.Add(oAmount);
            this.lstData.Add(oPrice1);
            this.lstData.Add(oPrice2);
        }
    }
}
