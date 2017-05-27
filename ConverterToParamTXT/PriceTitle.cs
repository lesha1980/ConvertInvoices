using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConverterToParamTXT
{
    public enum TypePrice
    { 
        PDV,
        None
    }
    public abstract class PriceTitle: DataInvoice
    {
        private TypePrice ePrice;
        public PriceTitle()
        {
            this.sNameData = "Цена";
            this.ePrice = ConverterToParamTXT.TypePrice.PDV;
        }
        public TypePrice TypePrice
        {
            get {
                return this.ePrice;
            }
            set {
                this.ePrice = value;
            }
        }
     
    }
}
