using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConverterToParamTXT
{
    public class ParamTXTFile
    {
        private String sFileName = String.Empty;
        private Stack<String> cBarCode;
        private Stack<String> cNameProduct;
        private Stack<String> cAmountProducts;
        private Stack<String> cPrice;
        private Stack<String> cPricePdv;
        private String sSeparator = String.Empty;
        private String sFileSource = String.Empty;

        public ParamTXTFile(String FileName, String sSeparator)
        {
            this.sFileName = FileName;
            this.sSeparator = sSeparator;
            this.cBarCode = new Stack<string>();
            this.cNameProduct = new Stack<string>();
            this.cPrice = new Stack<string>();
            this.cPricePdv = new Stack<string>();
            this.cAmountProducts = new Stack<string>();
        }

        public String FileSource
        {
            get {
                return this.sFileSource;
            }
            set {
                this.sFileSource = value;
            }
        }
        public String FileName
        {
            get {
                return this.sFileName;
            }
            set {
                this.sFileName = value;
            }
        }

        public String Separator
        {
            get {
                return this.sSeparator;
            }
            set {
                this.sSeparator = value;
            }
        }
        public Stack<String> BarCode
        {
            get {
                return this.cBarCode;
            }
            set {
                this.cBarCode = value;
            }
        }
        public Stack<String> NameProduct
        {
            get {
                return this.cNameProduct;
            }
            set {
                this.cNameProduct = value;
            }
        }
        public Stack<String> AmountProducts
        {
            get {
                return this.cAmountProducts;
            }
            set {
                this.cAmountProducts = value;
            }
        }
        public Stack<String> Price
        {
            get {
                return this.cPrice;
            }
            set {
                this.cPrice = value;
            }
        }
        public Stack<String> PricePDV
        {
            get {
                return this.cPricePdv;
            }
            set {
                this.cPricePdv = value;
            }
        }

     /*   public override string ToString()
        {
            String sInvoice = String.Empty;
            sInvoice += "Штрихкод" + this.sSeparator + "Наименование" + this.sSeparator + "Количество" + this.sSeparator + "Цена\n" + this.sSeparator + "Цена без ПДВ";

            while(this.cPrice.Peek() != null)
            {
                sInvoice += (this.cBarCode.Count != 0? this.cBarCode.Pop(): "") + this.sSeparator 
                    + (this.cNameProduct.Count != 0? this.cNameProduct.Pop(): "") + this.sSeparator
                    + (this.cAmountProducts.Count != 0? this.cAmountProducts.Pop(): "" )+ this.sSeparator 
                    + (this.cPrice.Count!= 0? this.cPrice.Pop(): "" )+ "\n";
            }
            
            return sInvoice;
        }*/
    }
}
