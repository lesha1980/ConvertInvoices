using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConverterToParamTXT
{
    public abstract class AmountTitle: DataInvoice
    {
        public AmountTitle()
        {
            this.sNameData = "Количество";
        }

       /* public override void ExtractData(int start, int end, System.Data.DataTable table)
        {
            int sum = 0;
            int ablem = 0;
            for (int i = start + 1; i <= table.Rows.Count - 1; i++ )
            {
                if ((table.Rows[i][this.nColIndex] as String) != null)
                {
                    ablem = 0;
                    try
                    {
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
                    sum++;
                }
                else {
                    if (ablem > 2)
                        break;
                    ablem++;
                    sum++;
                }
            }

            this.nCountSigns = sum;
        }*/
    }
}
