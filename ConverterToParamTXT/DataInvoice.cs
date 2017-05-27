using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Text.RegularExpressions;

namespace ConverterToParamTXT
{
    public abstract class DataInvoice
    {
        protected int nColIndex;
        protected int nColIndexPlus;
        protected int nColIndexMinus;
        protected String sNameData;
        protected List<String> lstData;
        protected int nCountSigns;
       // protected String pattern;

        public DataInvoice()
        {
            this.nColIndex = -1;
            this.nCountSigns = 0;
            this.lstData = new List<String>();
        }
      //  public abstract void SearchNameData(object[] row);
        public abstract void ExtractData(int start, int end, DataTable table);
        #region comment
        /* {
            int sum = 0;
            int ablem = 0;
            if (this is AmountTitle)
            {
                end = table.Rows.Count - 1;
            }
            else
            {
                end = end + start;
            }
            for (int i = start + 1; i <= end; i++)
            {
                if ((table.Rows[i][this.nColIndex] as String) != null)
                {
                    if (this is AmountTitle || this is PriceTitle)
                    {
                        ablem = 0;
                        try
                        {
                            String pr = table.Rows[i][this.nColIndex] as String;

                            if (i== start + 1 && (pr != null && this is PriceTitle))
                            {
                                    
                                    int ind = pr.IndexOf("ПДВ");
                                    int ind1 = pr.IndexOf("НДС");

                                    if (ind != -1 || ind1 != -1)
                                    {
                                        if (pr.IndexOf("без") != -1 || pr.IndexOf("бес") != -1)
                                        {
                                            this.ColIndex++;
                                            this.lstData.Add(null);
                                            continue;
                                        }
                                    }
                          
                                
                            }
                            float result = -1;
                            float.TryParse(table.Rows[i][this.nColIndex] as String, out result);
                            this.lstData.Add(table.Rows[i][this.nColIndex] as String);
                        }
                        catch (Exception e)
                        {
                            try
                            {
                                int result = -1;
                                int.TryParse(table.Rows[i][this.nColIndex] as String, out result);
                                this.lstData.Add(table.Rows[i][this.nColIndex] as String);

                            }
                            catch (Exception ex)
                            {
                                break;
                            }
                        }
                        if(this is AmountTitle)
                            sum++;
                    }
                    else if(this is BarCode)
                    {
                        try
                        {
                            long result = -1;
                            long.TryParse(table.Rows[i][this.nColIndex] as String, out result);
                            this.lstData.Add(table.Rows[i][this.nColIndex] as String);
                        }
                        catch (Exception e)
                        {
                           
                        }

                    }
                   else
                   {
                       
                           this.lstData.Add(table.Rows[i][this.nColIndex] as String);
                       
                   }
                }
                else
                {

                    if (this is ProductName)
                    {

                        String s1 = table.Rows[i][this.nColIndex - 1] as String;
                        String s2 = table.Rows[i][this.nColIndex + 1] as String;

                        if (s1 != null || s2 != null)
                        {
                            long a = -1;
                            double b = -1;
                            if (s1 != null)
                                if (!long.TryParse(s1, out a) || !double.TryParse(s1, out b))
                                {
                                    this.lstData.Add(s1);
                                }
                            if (s2 != null)
                                if (!long.TryParse(s2, out a) || !double.TryParse(s2, out b))
                                {
                                    this.lstData.Add(s2);
                                }
                        }
                        else
                            this.lstData.Add(null);

                    }
                    else
                    {

                        this.lstData.Add(null);
                    }
                    
                    if (this is AmountTitle)
                    {
                        sum++;
                        if (ablem > 2)
                            break;
                        ablem++;
                        
                    }
                }
            }
            if (this is AmountTitle)
                this.nCountSigns = sum;
            else
                this.nCountSigns = end;
        }*/
        #endregion comment
        public int CountSigns
        {
            get {
                return this.nCountSigns;
            }
            set {
                this.nCountSigns = value;
            }
        }
        public int ColIndex
        {
            get {
                return this.nColIndex;
            }
            set {
                this.nColIndex = value;
            }
        }
        public String NameData
        {
            get {
                return this.sNameData;
            }
        }
        public String this[int index]
        {
            get {
                return this.lstData[index];
            }
            set {
                this.lstData[index] = value;
            }
        }
        public void addData(String str)
        {
            this.lstData.Add(str);
        }
        public int CountData
        {
            get {
                return this.lstData.Count;
            }
        }

        public int ColIndexPlus
        {
            get {
                return this.nColIndexPlus;
            }
            set {
                this.nColIndexPlus = value;
            }
        }

        public int ColIndexMinus
        {
            get {
                return this.nColIndexMinus;
            }
            set {
                this.nColIndexMinus = value;
            }
        }

    }
}
