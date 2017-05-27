using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Text.RegularExpressions;
using ConverterToParamTXT;

namespace ComXLS
{
    public enum HeaderTitles
    {
         BarCode,
         Product,
         Price,
         Amount
    }
    public class SearcherTitlesInvoice
    {
        private List<DataInvoice> lstBarCodeCol;
        private List<DataInvoice> lstProductCol;
        private List<DataInvoice> lstPriceCol;
        private List<DataInvoice> lstAmountCol;
        private List<InvoiceAbstract> lstInvoices;
        private LogInf oLog;

        private String pattern1;
        private String pattern2;
        private String pattern3;
        private String pattern4;
        private String pattern5;
        private String pattern6;

        public SearcherTitlesInvoice()
        {
            this.lstBarCodeCol = new List<DataInvoice>();
            this.lstPriceCol = new List<DataInvoice>();
            this.lstProductCol = new List<DataInvoice>();
            this.lstAmountCol = new List<DataInvoice>();
            this.lstInvoices = new List<InvoiceAbstract>();
            this.oLog = new LogInf();

            this.pattern1 = @"((^)((\b)сертификат(\b)(\s)*№)($)|(^)(\b)((штрих((\s)*|(\W)*)){0,1}код)(\b)($))|(^)(\b)((\s)*шк(\s)*)(\b)($)|((^)(\b)код(\b)($))";
            this.pattern2 = @"(\b)((назва(ние){0,1})|(на(и|й|\w)мен(о|у)ван(ие|ня))|(товар))(\b)";
            this.pattern3 = @"(^)((\s)*(\b)((кол(\.){0,1}(((\w)+|(\s)*|(\W)+)во){0,1})|(к(\w)л(\.){0,1}((ь)*(\s)*(\W)*(\s)*((к(\w))*(\S)*с)*ть){0,1}((\s)+(одиниц(ь){0,1})){0,1})|(к((\s)*|(\W)+)*(с){0,1}ть)|(к(\w)льк(\W)*(\s)*)ть)|(коли(\W)*(\s)*чество)(\b)(\s)*)($)";
            this.pattern4 = @"(((^)(\s)*(\b)(ц(\w){1}на)(\b)(\s)*(в(\s)*(грн)(\.)*){0,1}(за(\s)*шт(\.){0,1}){0,1}(\s)*(([зс]|(без|бес))(\s)*(НДС|ПДВ)*){0,1}((\W){0,1}(\s)*(грн){0,1}(\W){0,1}){0,1}($))|(^)(([зс]|(без|бес))(\s)*(НДС|ПДВ)*){0,1}($))";
            this.pattern5 = @"(^)([№N]{0,1}(\\n)*((п(\W){0,1}п)*))($)";
            this.pattern6 = @"(^)((\b)сертификат(\b)(\s)*№)($)";
            
        }
        public int CountInv
        {
            get {
                return this.lstInvoices.Count;
            }
        }
        public InvoiceAbstract this[int index]
        {
            get {
                return this.lstInvoices[index];
            }
        }
        private Regex GetRegex(HeaderTitles title)
        {
            Regex reg = null;
            switch (title)
            {
                case HeaderTitles.BarCode:
                    reg = new Regex(pattern1, RegexOptions.IgnoreCase);
                    break;
                case HeaderTitles.Price:
                    reg = new Regex(pattern4, RegexOptions.IgnoreCase);
                    break;
                case HeaderTitles.Product:
                    reg = new Regex(pattern2, RegexOptions.IgnoreCase);
                    break;
                case HeaderTitles.Amount:
                    reg = new Regex(pattern3, RegexOptions.IgnoreCase);
                    break;
            }
            return reg;
        }
        private void AddSearchingValue(HeaderTitles title, int index, String sNameCode = "Штрихкод", TypePrice eType = TypePrice.PDV)
        {
            DataInvoice oData = null;
            switch (title)
            {
                case HeaderTitles.BarCode:
                    oData = new BarCodeXLS();
                    oData.ColIndex = index;
                    (oData as BarCode).sNameCode = sNameCode;
                    this.lstBarCodeCol.Add(oData);
                    break;
                case HeaderTitles.Amount:
                    oData = new AmountTitleXLS();
                    oData.ColIndex = index;
                    this.lstAmountCol.Add(oData);
                    break;
                case HeaderTitles.Price:
                    oData = new PriceTitleXLS();
                    oData.ColIndex = index;
                    (oData as PriceTitle).TypePrice = eType;
                    this.lstPriceCol.Add(oData);
                    break;
                case HeaderTitles.Product:
                    oData = new ProductNameXLS();
                    oData.ColIndex = index;
                    this.lstProductCol.Add(oData);
                    break;

            }
        }
        public void SearchTitles(object[] rows, HeaderTitles title, DataTable table, int index)
        {
            Regex reg = this.GetRegex(title);
          

            for (int i = 0; i <= rows.Length - 1; i++)
            {
                if ((rows[i] as String) != null)
                {
                    if (reg.IsMatch(rows[i] as String))
                   {
                    
                       if (title == HeaderTitles.Price)
                       {

                           TypePrice eType = TypePrice.PDV;
                           String s = rows[i] as String;
                           int ind = s.IndexOf("ПДВ");
                           int ind1 = s.IndexOf("НДС");

                           if (ind != -1 || ind1 != -1)
                           {
                               if (s.IndexOf("без") != -1 || s.IndexOf("бес") != -1)
                                   eType = TypePrice.None;
                               else
                               {
                                   if (s.IndexOf("с") != -1 || s.IndexOf("з") != -1)
                                   {
                                       eType = TypePrice.PDV;
                                       if (lstPriceCol.Count != 0)
                                       {
                                           (lstPriceCol[0] as PriceTitle).TypePrice = TypePrice.None;
                                       }
                                   }
                               }
                               this.AddSearchingValue(title, i, null, eType);
                           }
                           else
                           {
                               if (this.lstPriceCol.Count == 0)
                               {
                                   this.AddSearchingValue(title, i, null, eType);
                               }
                               else {
                                   if ((this.lstPriceCol[0] as PriceTitle).TypePrice == TypePrice.None)
                                   {
                                       this.lstPriceCol.Clear();
                                       this.AddSearchingValue(title, i, null, eType);
                                   }

                               }
                           }
                       }
                       else{
                           if (title == HeaderTitles.BarCode)
                           {
                               try
                               {
                                   int idx = 1;
                                   String sNameBarCode = rows[i] as String;
                                   while(true)
                                   {
                                      String s = table.Rows[index + idx][i] as String;
                                      if (s != null)
                                      {
                                          s = s.Trim();
                                          if (!s.Equals(String.Empty))
                                          {
                                              
                                              if (s.LastIndexOf(',') != -1)
                                              {
                                                  int sep = s.LastIndexOf(',');
                                                  s = s.Substring(0, sep);
                                              }

                                              long ls = long.Parse(s);
                                              if (ls > -1)
                                                  break;
                                          }
                                      }
                                      else if(rows[i + 1] as String == null) {
                                          s = table.Rows[index + idx][i + 1] as String;
                                          if (s != null)
                                          {
                                              s = s.Trim();
                                              if (s.LastIndexOf(',') != -1)
                                              {
                                                  int sep = s.LastIndexOf(',');
                                                  s = s.Substring(0, sep);
                                              }

                                              long ls = long.Parse(s);
                                              if (ls > -1)
                                              {
                                                  i++;
                                                  break;
                                              }
                                          }
                                      
                                      }

                                          idx++;
                                   }
                                   if (this.lstBarCodeCol.Count == 0)
                                       this.AddSearchingValue(title, i, sNameBarCode);
                                   if (this.lstBarCodeCol.Count != 0)
                                   {
                                      String sName = (this.lstBarCodeCol[0] as BarCode).sNameCode;
                                      if (!sName.Equals(sNameBarCode) && sName.Equals("Код"))
                                      {
                                          this.lstBarCodeCol.Clear();
                                          this.AddSearchingValue(title, i, sNameBarCode);
                                      }
                                   }
                               }
                               catch(Exception e)
                               {
                               
                               }
                              
                           }
                           else if(title == HeaderTitles.Amount)
                           {
                               String s = rows[i] as String;

                               if (s.IndexOf("головок") != -1)
                               {
                                   continue;
                               }
                               this.AddSearchingValue(title, i);
                          
                           }
                           else
                           {
                               this.AddSearchingValue(title, i);
                           }
                           
                       }
                   }
                }
            }

           
        }
        public void CountInvoices(object[] rows, int indrow, DataTable table)
        {
            if (this.lstProductCol.Count == 1)
            {
                try
                {
                    List<DataInvoice> lst = new List<DataInvoice>();
                    if (this.lstBarCodeCol.Count > 0)
                        lst.Add(this.lstBarCodeCol[0]);
                    lst.Add(this.lstProductCol[0]);
                    lst.Add(this.lstAmountCol[0]);
                    foreach (PriceTitle p in this.lstPriceCol)
                    {
                        lst.Add(p);
                    }
                    this.lstInvoices.Add(new InvoiceCls(lst));
                }
                catch(Exception e)
                {
                    this.oLog.logDataError("Файл не обработан: " + e.Message, e.StackTrace);
                }
            }
            else if (this.lstProductCol.Count == 2)
            {
                if (this.lstBarCodeCol.Count == 2)
                {
                    try
                    {
                        List<DataInvoice> lst = new List<DataInvoice>();

                        lst.Add(this.lstBarCodeCol[0]);
                        lst.Add(this.lstProductCol[0]);
                        lst.Add(this.lstAmountCol[0]);

                        if (this.lstPriceCol.Count == 2)
                        {
                            lst.Add(this.lstPriceCol[0]);
                        }
                        else if (this.lstPriceCol.Count == 4)
                        {
                            lst.Add(this.lstPriceCol[0]);
                            lst.Add(this.lstPriceCol[1]);
                        }

                        this.lstInvoices.Add(new InvoiceCls(lst));

                        List<DataInvoice> lst1 = new List<DataInvoice>();

                        lst1.Add(this.lstBarCodeCol[1]);
                        lst1.Add(this.lstProductCol[1]);
                        lst1.Add(this.lstAmountCol[1]);

                        if (this.lstPriceCol.Count == 2)
                        {
                            lst1.Add(this.lstPriceCol[1]);
                        }
                        else if (this.lstPriceCol.Count == 4)
                        {
                            lst1.Add(this.lstPriceCol[2]);
                            lst1.Add(this.lstPriceCol[3]);
                        }

                        this.lstInvoices.Add(new InvoiceCls(lst1));
                    }
                    catch(Exception e)
                    {
                        this.oLog.logDataError("Файл не обработан: " + e.Message, e.StackTrace);
                    }
                }
                else if (this.lstBarCodeCol.Count == 1)
                {
                    BarCode oBarCode = this.lstBarCodeCol[0] as BarCode;
                    ProductName oProduct1 = this.lstProductCol[0] as ProductName;
                    ProductName oProduct2 = this.lstProductCol[1] as ProductName;

                    if (oBarCode.ColIndex < oProduct1.ColIndex)
                    {
                        try
                        {
                            List<DataInvoice> lst = new List<DataInvoice>();

                            lst.Add(this.lstBarCodeCol[0]);
                            lst.Add(this.lstProductCol[0]);
                            lst.Add(this.lstAmountCol[0]);

                            if (this.lstPriceCol.Count == 2)
                            {
                                lst.Add(this.lstPriceCol[0]);
                            }
                            else if (this.lstPriceCol.Count == 4)
                            {
                                lst.Add(this.lstPriceCol[0]);
                                lst.Add(this.lstPriceCol[1]);
                            }

                            this.lstInvoices.Add(new InvoiceCls(lst));

                            List<DataInvoice> lst1 = new List<DataInvoice>();
                            lst1.Add(this.lstProductCol[1]);
                            lst1.Add(this.lstAmountCol[1]);

                            if (this.lstPriceCol.Count == 2)
                            {
                                lst1.Add(this.lstPriceCol[1]);
                            }
                            else if (this.lstPriceCol.Count == 4)
                            {
                                lst1.Add(this.lstPriceCol[2]);
                                lst1.Add(this.lstPriceCol[3]);
                            }

                            this.lstInvoices.Add(new InvoiceCls(lst1));
                        }
                        catch (Exception e)
                        {
                            this.oLog.logDataError("Файл не обработан: " + e.Message, e.StackTrace);
                        }
                    }
                    else if (oBarCode.ColIndex > oProduct2.ColIndex)
                    {
                        try
                        {
                            List<DataInvoice> lst = new List<DataInvoice>();
                            lst.Add(this.lstProductCol[0]);
                            lst.Add(this.lstAmountCol[0]);

                            if (this.lstPriceCol.Count == 2)
                            {
                                lst.Add(this.lstPriceCol[0]);
                            }
                            else if (this.lstPriceCol.Count == 4)
                            {
                                lst.Add(this.lstPriceCol[0]);
                                lst.Add(this.lstPriceCol[1]);
                            }

                            this.lstInvoices.Add(new InvoiceCls(lst));

                            List<DataInvoice> lst1 = new List<DataInvoice>();

                            lst1.Add(this.lstBarCodeCol[0]);
                            lst1.Add(this.lstProductCol[1]);
                            lst1.Add(this.lstAmountCol[1]);

                            if (this.lstPriceCol.Count == 2)
                            {
                                lst1.Add(this.lstPriceCol[1]);
                            }
                            else if (this.lstPriceCol.Count == 4)
                            {
                                lst1.Add(this.lstPriceCol[2]);
                                lst1.Add(this.lstPriceCol[3]);
                            }

                            this.lstInvoices.Add(new InvoiceCls(lst1));
                        }
                        catch(Exception e)
                        {
                            this.oLog.logDataError("Файл не обработан: " + e.Message, e.StackTrace);
                        }
                    }
                    else if (oBarCode.ColIndex > oProduct1.ColIndex && oBarCode.ColIndex < oProduct2.ColIndex)
                    {
                        int i = oBarCode.ColIndex;
                        Regex reg = new Regex(pattern5, RegexOptions.IgnoreCase);
                        bool found = false;
                        int result1 = -1;
                        int result2 = -1;
                        for (int j = i - 1; j >= 0; j--)
                        {
                            if ((rows[j] as String) != null)
                            {
                                String s = rows[j] as String;

                                if (reg.IsMatch(s))
                                {
                                    found = true;
                                    result1 = j;
                                    break;
                                }
                            }
                        }
                        if (!found)
                        {
                            for (int j = i + 1; j <= rows.Length - 1; j++)
                            {
                                if ((rows[j] as String) != null)
                                {
                                    String s = rows[j] as String;

                                    if (reg.IsMatch(s))
                                    {
                                        found = true;
                                        result2 = j;
                                        break;
                                    }
                                }
                            }
                        }
                      
                        if (!found)
                        {
                            for (int j = i - 1; j >= 0; j--)
                            {
                                if ((rows[j] as String) == null)
                                {
                                    for (int k = indrow + 1; k <= indrow + 6; k++)
                                    {
                                        if ((table.Rows[k][j] as String) is String)
                                        {
                                            try
                                            {
                                                int.TryParse((table.Rows[k][j] as String), out result1);
                                                found = true;
                                                if(result1 > 0)
                                                    result1 = j;
                                                break;
                                            }
                                            catch(Exception e)
                                            {
                                                found = true;
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (found)
                                    break;
                            }
                          if(!found)
                          {
                            for (int j = i + 1; j <= rows.Length - 1; j++)
                            {
                                if ((rows[j] as String) == null)
                                {
                                    for (int k = indrow + 1; k <= indrow + 6; k++)
                                    {
                                        if ((table.Rows[k][j] as String) is String)
                                        {
                                            try
                                            {
                                                int.TryParse((table.Rows[k][j] as String), out result2);
                                                found = true;
                                                if(result2 > 0)
                                                    result2 = j;
                                                break;
                                            }
                                            catch (Exception e)
                                            {
                                                found = true;
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (found)
                                    break;
                            }
                          }


                        }

                        if (found)
                        {
                            if (result1 > 0 && oBarCode.ColIndex > result1)
                            {
                                try
                                {
                                    List<DataInvoice> lst = new List<DataInvoice>();
                                    lst.Add(this.lstProductCol[0]);
                                    lst.Add(this.lstAmountCol[0]);

                                    if (this.lstPriceCol.Count == 2)
                                    {
                                        lst.Add(this.lstPriceCol[0]);
                                    }
                                    else if (this.lstPriceCol.Count == 4)
                                    {
                                        lst.Add(this.lstPriceCol[0]);
                                        lst.Add(this.lstPriceCol[1]);
                                    }

                                    this.lstInvoices.Add(new InvoiceCls(lst));

                                    List<DataInvoice> lst1 = new List<DataInvoice>();

                                    lst1.Add(this.lstBarCodeCol[0]);
                                    lst1.Add(this.lstProductCol[1]);
                                    lst1.Add(this.lstAmountCol[1]);

                                    if (this.lstPriceCol.Count == 2)
                                    {
                                        lst1.Add(this.lstPriceCol[1]);
                                    }
                                    else if (this.lstPriceCol.Count == 4)
                                    {
                                        lst1.Add(this.lstPriceCol[2]);
                                        lst1.Add(this.lstPriceCol[3]);
                                    }

                                    this.lstInvoices.Add(new InvoiceCls(lst1));
                                }
                                catch (Exception e)
                                {
                                    this.oLog.logDataError("Файл не обработан: " + e.Message, e.StackTrace);
                                }
                            }
                            if (result2 > 0 && oBarCode.ColIndex < result2)
                            {
                                try
                                {
                                    List<DataInvoice> lst = new List<DataInvoice>();

                                    lst.Add(this.lstBarCodeCol[0]);
                                    lst.Add(this.lstProductCol[0]);
                                    lst.Add(this.lstAmountCol[0]);

                                    if (this.lstPriceCol.Count == 2)
                                    {
                                        lst.Add(this.lstPriceCol[0]);
                                    }
                                    else if (this.lstPriceCol.Count == 4)
                                    {
                                        lst.Add(this.lstPriceCol[0]);
                                        lst.Add(this.lstPriceCol[1]);
                                    }

                                    this.lstInvoices.Add(new InvoiceCls(lst));

                                    List<DataInvoice> lst1 = new List<DataInvoice>();
                                    lst1.Add(this.lstProductCol[1]);
                                    lst1.Add(this.lstAmountCol[1]);

                                    if (this.lstPriceCol.Count == 2)
                                    {
                                        lst1.Add(this.lstPriceCol[1]);
                                    }
                                    else if (this.lstPriceCol.Count == 4)
                                    {
                                        lst1.Add(this.lstPriceCol[2]);
                                        lst1.Add(this.lstPriceCol[3]);
                                    }

                                    this.lstInvoices.Add(new InvoiceCls(lst1));
                                }
                                catch (Exception e)
                                {
                                    this.oLog.logDataError("Файл не обработан: " + e.Message, e.StackTrace);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
        
}
