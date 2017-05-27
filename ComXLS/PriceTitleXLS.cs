using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConverterToParamTXT;
using System.Data;

namespace ComXLS
{
    public class PriceTitleXLS: PriceTitle
    {
        public PriceTitleXLS()
        { 
        
        }

        public override void ExtractData(int start, int end, DataTable table)
        {
            end = end + start;
            
            for (int i = start + 1; i <= end; i++)
            {
                if ((table.Rows[i][this.nColIndex] as String) != null)
                {
                        try
                        {
                            String pr = table.Rows[i][this.nColIndex] as String;

                            if (i == start + 1 && pr != null)
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

                        }
                    
                }
                else
                {
                       this.lstData.Add(null);
                }
            }
             this.nCountSigns = end;
            #region comment
            /* int sum = 0;
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

                            if (i == start + 1 && (pr != null && this is PriceTitle))
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
                        if (this is AmountTitle)
                            sum++;
                    }
                    else if (this is BarCode)
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
                this.nCountSigns = end;*/
            #endregion comment
        }
    }
}
