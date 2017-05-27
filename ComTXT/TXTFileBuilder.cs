using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConverterToParamTXT;
using System.IO;

namespace ComTXT
{
    public class TXTFileBuilder: FileBuilder
    {
       // private ProcessedFile oFile;
        private String strResult = String.Empty;
       // private List<ParamTXTFile> lstParamTXTFile;
        private List<String> lstLines;
        private Stack<DataInvoice> lstBarCode;
        private Stack<DataInvoice> lstProduct;
        private Stack<DataInvoice> lstAmount;
        private Stack<DataInvoice> lstPrice;
        private InvoiceAbstract oInvoice;

        public override void Initialize(String sPath)
        {
            this.oFile = new TXTProcessed(sPath);
           // this.oFile = poFile;
            this.oFile.ReadFile();
            this.strResult = (this.oFile as TXTProcessed).StrRead;
            this.lstParamTXTFile = new List<ParamTXTFile>();
            this.lstLines = new List<string>();
            

            base.Initialize(sPath);

        }

        public override List<ParamTXTFile> Start()
        {
            this.getListLines(this.strResult);
           
          if(this.lstLines.Count > 0)
          {
            FileInfo info = new FileInfo(this.oFile.FileName);
         
            int index = info.Name.LastIndexOf(info.Extension);
            String filename = info.Name.Substring(0, index);
            string resfile = this.ParentPath.FullName + "\\" + filename + ".txt";  
          if(!File.Exists(resfile)){
            ParamTXTFile txtfile = new ParamTXTFile(resfile, "@");
            txtfile.FileSource = this.oFile.FileName;
            for (int i = this.lstLines.Count - 1; i >= 0; i-- )
            {
               if(this.lstLines[i] != String.Empty)
                this.transformLine(this.lstLines[i], txtfile);
            }
            this.lstParamTXTFile.Add(txtfile);
            }
          }

          return this.lstParamTXTFile;
            
        }
        private void transformLine(String str, ParamTXTFile txtfile)
        {
            int ind = str.IndexOf("@");
            int ind1 = str.IndexOf("@", ind + 1);
            str = str.Remove(ind1, 1);

           // DataInvoice oBarCode = new TXTBarCode();
            String strBC = str.Substring(0, ind);
            txtfile.BarCode.Push(strBC);
            
           // oBarCode.addData(strBC);
           // this.lstBarCode.Push(oBarCode);
            str = str.Substring(ind + 1);

           // DataInvoice oProduct = new TXTProductName();
            String strPr = str.Substring(0, str.IndexOf("@"));
            txtfile.NameProduct.Push(strPr);
           // oProduct.addData(strPr);
           // this.lstProduct.Push(oProduct);

           // DataInvoice oPrice = new TXTPriceTitle();
            while (str.LastIndexOf("@") == str.Length - 1)
            {
                str = str.Remove(str.LastIndexOf("@"), 1);
            }

            int lind = str.LastIndexOf("@");
            String strPrice = str.Substring(lind + 1);
           // oPrice.addData(strPrice);
           // this.lstPrice.Push(oPrice);
            txtfile.PricePDV.Push(strPrice);
            str = str.Substring(0, lind);

           // DataInvoice oAmount = new TXTAmountTitle();
            String strAm = str.Substring(str.LastIndexOf("@") + 1);
           // oAmount.addData(strAm);
          //  this.lstAmount.Push(oAmount);
            txtfile.AmountProducts.Push(strAm);
            

        }
        private void getListLines(String str)
        {
            lstLines.Add("");
            while (str.IndexOf('\n') != -1)
            {
                String sub = str.Substring(0, str.IndexOf('\n') - 1);
                str = str.Substring(str.IndexOf('\n') + 1);
                lstLines.Add(sub);
            }
            lstLines.Add(str);
        }
    }
}
