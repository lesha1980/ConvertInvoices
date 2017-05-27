using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows;
using System.IO;
using ConverterToParamTXT;


namespace ComXLS
{
    public class XLSFileBuilder: FileBuilder
    {
      
        private List<DataSet> lstDataSet;
        private List<DataSet> lstResultDataSet;
        private List<DataSet> lstWorkingDataSet;
        private String priceFormat = String.Empty;
        private List<String> lstNameInvoices;
        private Hashtable htInvoiceHeaders;
        private List<Hashtable> lsthtRangeBorders;
        private Queue<SearchingColumnRows> qHeaderRow;
        private List<SearchingColumnRows> lstColR;
        private List<InvoiceAbstract> lstInvoices;
        private ProcessedFile oFile;
        private static Mutex oMutex = new Mutex();
        private LogInf oLog;
       // private List<ParamTXTFile> lstParamTXTFile;
        
       // private Queue<DataColumn> qColumns;
       // private Hashtable htRangeBorder;
        private int nInvoicesCountBySheet;

        public List<ParamTXTFile> getResult()
        {
            return this.lstParamTXTFile;
        }
        public override void Initialize(String sPath)
        {
            this.oFile = new XLSProcessed(sPath);
           // this.oFile = poFile;
            this.oFile.ReadFile();
            this.lstDataSet = (this.oFile as XLSProcessed).LstDataSet;
            this.lstResultDataSet = new List<DataSet>();
            this.lstWorkingDataSet = new List<DataSet>();
            this.lstNameInvoices = new List<String>();
            this.htInvoiceHeaders = new Hashtable();
            this.qHeaderRow = new Queue<SearchingColumnRows>();
            this.lstColR = new List<SearchingColumnRows>();
            this.lstInvoices = new List<InvoiceAbstract>();
            this.lstParamTXTFile = new List<ParamTXTFile>();
            this.oLog = new LogInf();
           

       
           // this.qColumns = new Queue<DataColumn>();
          //  this.htRangeBorder = new Hashtable();
          //  this.htRangeBorder.Add("colstart", null);
          //  this.htRangeBorder.Add("colend", null);
         //   this.htRangeBorder.Add("rstart", null);
         //   this.htRangeBorder.Add("rend", null);
            this.nInvoicesCountBySheet = 0;
            base.Initialize(sPath);
        }

        public override List<ParamTXTFile> Start()
        {
           // oMutex.WaitOne();
            this.PreProcessNameInvoice();
            if (this.lstWorkingDataSet.Count == 0)
            {
                this.NameInvoiceNotExist();
               if(this.lstWorkingDataSet.Count == 0)
                return null;
            }
            for (int i = 0; i <= this.lstWorkingDataSet.Count - 1; i++)
            {
               
                this.lstResultDataSet.Add(this.CreateTargetDataSet(this.lstWorkingDataSet[i]));
                this.lstColR.Clear();
            }
            this.CreateTXTParam();
          // oMutex.ReleaseMutex();
           return this.lstParamTXTFile;
        }

        private DataSet CreateTargetDataSet(DataSet oDataSet)
        {
            int index = -1;
            DataSet oResultDataSet = new DataSet();
            //DataTable oResultDataTable = new DataTable();
           
           // int[] arrIndex = new int[20];
            foreach (DataTable table in oDataSet.Tables)
            {
               /* foreach(var column in table.Columns.Cast<DataColumn>().ToArray())
                {
                   if(table.AsEnumerable().All(dr => dr.IsNull(column)))
                   {
                       table.Columns.Remove(column);
                   }
                }*/
                foreach (DataColumn column in table.Columns)
                {
                   this.SearchHeaderInvoice(column, table);
                   for (int i = 0; i <= this.qHeaderRow.Count - 1; i++)
                   {
                       SearchingColumnRows colr = this.qHeaderRow.Dequeue();
                       SearcherTitlesInvoice sti = new SearcherTitlesInvoice();
                       sti.SearchTitles(colr.Row.ItemArray, HeaderTitles.BarCode, table, colr.IdxRow);
                       sti.SearchTitles(colr.Row.ItemArray, HeaderTitles.Product, table, colr.IdxRow);
                       sti.SearchTitles(colr.Row.ItemArray, HeaderTitles.Amount, table, colr.IdxRow);
                       sti.SearchTitles(colr.Row.ItemArray, HeaderTitles.Price, table, colr.IdxRow);
                       sti.CountInvoices(colr.Row.ItemArray, table.Rows.IndexOf(colr.Row), table);

                       int end = 0;
                       for (int k = 0; k <= sti.CountInv - 1; k++)
                       {
                           for (int j = 0; j <= sti[k].CountDI - 1; j++)
                           {
                               if (sti[k][j] is AmountTitle)
                               {
                                   sti[k][j].ExtractData(colr.IdxRow, 0, table);
                                   end = sti[k][j].CountSigns;
                               }
                           }
                           for (int j = 0; j <= sti[k].CountDI - 1; j++)
                           {
                               if (!(sti[k][j] is AmountTitle))
                               {
                                   sti[k][j].ExtractData(colr.IdxRow, end, table);
                               }
                           }

                       }
                       for (int j = 0; j <= sti.CountInv - 1; j++)
                       {

                               this.lstInvoices.Add(sti[j]);

                       }
                     


                   }

                }
              
                /*  int i = 0;
                foreach (DataRow oHeaderRow in listoHeaderRow)
                {
                    arrIndex[i] = table.Rows.IndexOf(oHeaderRow);
                    i++;
                }
                Array.Sort(arrIndex);
                for (int j = 0; j <= arrIndex.Length - 1; j++ )
                {
                    if (arrIndex[j] != 0)
                    {
                        index = arrIndex[j];
                        break;
                    }
                }*/
       
                //  DataRow HeaderRow = table.Rows[index];
              
            }
            

            return oResultDataSet;
        }
        private void CreateTXTParam()
        {

            int i = 1;
            foreach (InvoiceAbstract inv in this.lstInvoices)
            {
                FileInfo info = new FileInfo(this.oFile.FileName);
                int index = info.Name.LastIndexOf(info.Extension);
                String filename = info.Name.Substring(0, index);
                ParamTXTFile txtfile = new ParamTXTFile(this.ParentPath.FullName + "\\" + filename + "_" + i + ".txt", "@");
                txtfile.FileSource = this.oFile.FileName;

                Stack<String> name = new Stack<string>();
                Stack<String> barcod = new Stack<string>();
                Stack<String> amounts = new Stack<string>();
                Stack<String> prpdv = new Stack<string>();
                Stack<String> pr = new Stack<string>();
                for (int j = 0; j <= inv.CountDI - 1; j++ )
                {
                    for (int d = inv[j].CountData - 1; d >= 0; d--)
                    {
                        if (inv[j] is ProductName)
                        {
                            name.Push(inv[j][d]);
                        }
                        if (inv[j] is BarCode)
                        {
                            barcod.Push(inv[j][d]);
                        }
                        if (inv[j] is AmountTitle)
                        {
                            amounts.Push(inv[j][d]);
                        }
                        if (inv[j] is PriceTitle)
                        {
                            if ((inv[j] as PriceTitle).TypePrice == TypePrice.PDV)
                            {
                                prpdv.Push(inv[j][d]);
                            }
                            else {
                                pr.Push(inv[j][d]);
                            }
                        }
                    }
                    
                    
                 
                }
                if(name.Count != 0 && name.Count < amounts.Count)
                {
                    int dif = amounts.Count - name.Count;

                    for (int d = 0; d <= dif - 1; d++)
                    {
                        name.Push(null);
                    }
                }
                if (barcod.Count != 0 && barcod.Count < amounts.Count)
                {
                    int dif = amounts.Count - barcod.Count;

                    for (int d = 0; d <= dif - 1; d++)
                    {
                        barcod.Push(null);
                    }
                }
                if (pr.Count != 0 && pr.Count < amounts.Count)
                {
                    int dif = amounts.Count - pr.Count;

                    for (int d = 0; d <= dif - 1; d++)
                    {
                        pr.Push(null);
                    }
                }
                if (prpdv.Count != 0 && prpdv.Count < amounts.Count)
                {
                    int dif = amounts.Count - prpdv.Count;

                    for (int d = 0; d <= dif - 1; d++)
                    {
                        prpdv.Push(null);
                    }
                }
               
               
                txtfile.NameProduct = name;
                txtfile.BarCode = barcod;
                txtfile.AmountProducts = amounts;
                txtfile.Price = pr;
                txtfile.PricePDV = prpdv;

                this.lstParamTXTFile.Add(txtfile);
                i++;
            }

              
        }
        private void NameInvoiceNotExist()
        {
            DataRow[] rows = null;
            if (lstDataSet.Count == 1)
            {
                
                if (lstDataSet[0].Tables["Table"].Rows.Count > 0)
                    foreach (DataColumn column in lstDataSet[0].Tables["Table"].Columns)
                    {
                        
                        try
                        {
                            //  String sExpression = column.ColumnName + " LIKE '*наименование*' OR " + column.ColumnName + " LIKE '*назва*' OR " + column.ColumnName + " LIKE '*товар*' ";
                            String sExpression = "(" + column.ColumnName + " LIKE 'штрих*' OR " + column.ColumnName + " LIKE 'код') AND " + column.ColumnName + " NOT LIKE '*уктзд*'";
                            rows = lstDataSet[0].Tables["Table"].Select(sExpression);
                            if (rows.Length > 0)
                            {
                                if (!this.lstWorkingDataSet.Contains(this.lstDataSet[0]))
                                    this.lstWorkingDataSet.Add(this.lstDataSet[0]);

                            }
                        }
                        catch (EvaluateException e)
                        {
                            return;
                        }
          
                    }
            }
            else
            {
                foreach (DataSet data in lstDataSet)
                {
                    if (data.Tables["Table"].Rows.Count > 0)
                        foreach (DataColumn column in data.Tables["Table"].Columns)
                        {
                            
                            try
                            {
                                //  String sExpression = column.ColumnName + " LIKE '*наименование*' OR " + column.ColumnName + " LIKE '*назва*' OR " + column.ColumnName + " LIKE '*товар*' ";
                                String sExpression = "(" + column.ColumnName + " LIKE 'штрих*' OR " + column.ColumnName + " LIKE 'код') AND " + column.ColumnName + " NOT LIKE '*уктзд*'";
                                rows = lstDataSet[0].Tables["Table"].Select(sExpression);
                                if (rows.Length > 0)
                                {
                                    if (!this.lstWorkingDataSet.Contains(data))
                                        this.lstWorkingDataSet.Add(data);

                                }
                                
                            }
                            catch (EvaluateException e)
                            {
                                return;
                            }
          
                        }
                }
            }

           

        }
        private void PreProcessNameInvoice()
        {
            
            if(lstDataSet.Count == 1)
            {
               if(lstDataSet[0].Tables["Table"].Rows.Count > 0)
                foreach(DataColumn column in lstDataSet[0].Tables["Table"].Columns)
                {
                    if (this.SearchNameInvoice(column, lstDataSet[0].Tables["Table"]))
                    {
                    if (!this.lstWorkingDataSet.Contains(this.lstDataSet[0]))
                        this.lstWorkingDataSet.Add(this.lstDataSet[0]);
                    }


                }
            }
            else
            {
               foreach(DataSet data in lstDataSet)
               {
                   try
                   {
                       if (data.Tables["Table"].Rows.Count > 0)
                           foreach (DataColumn column in data.Tables["Table"].Columns)
                           {
                               if (this.SearchNameInvoice(column, data.Tables["Table"]))
                               {
                                   if (!this.lstWorkingDataSet.Contains(data))
                                       this.lstWorkingDataSet.Add(data);
                               }
                           }
                   }
                   catch(Exception e)
                   {
                       this.oLog.logDataError(e.Message, e.StackTrace);
                   }
               }
            }

            
        }
        private bool SearchNameInvoice(DataColumn column, DataTable table)
        {
            try
            {
                String sExpression = column.ColumnName + " LIKE '*накладна*' OR " + column.ColumnName + " LIKE '*накладная*' OR " + column.ColumnName + " LIKE '*фактура*' OR " + column.ColumnName + " LIKE '*ТТН*'";
                DataRow[] rows = table.Select(sExpression);

                if (rows.Length > 0)
                {
                    for (int i = 0; i <= rows.Length - 1; i++)
                    {
                        String invoice = rows[i].Field<string>(column);
                        this.lstNameInvoices.Add(invoice);
                        this.nInvoicesCountBySheet++;
                        
                    }
                  
                    return true;
                }
            }
            catch(Exception e)
            {
                //MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false;
        }
        private void SearchHeaderInvoice(DataColumn column, DataTable table)
        {
            DataRow[] rows = null;
            try
            {
                //  String sExpression = column.ColumnName + " LIKE '*наименование*' OR " + column.ColumnName + " LIKE '*назва*' OR " + column.ColumnName + " LIKE '*товар*' ";
                String sExpression = "(" + column.ColumnName + " LIKE 'штрих*' OR " + column.ColumnName + " LIKE '*наименование*' OR " + column.ColumnName + " LIKE 'код') AND " + column.ColumnName + " NOT LIKE '*уктзд*'";
                rows = table.Select(sExpression);
            }
            catch(EvaluateException e)
            {
                return;
            }
            if (rows.Length == 0)
            {
                String sExpression1 = column.ColumnName + " LIKE 'сертификат*'";
                rows = table.Select(sExpression1);

            }
            foreach(DataRow row in rows)
            {

                object[] data = row.ItemArray;
                
                int index = table.Rows.IndexOf(row);
                int i = 1;
                while (true)
                {

                    try
                    {
                        if ((table.Rows[index + i][column] as String) != null)
                        {
                            long digl = -1;
                            try
                            {
                                long.TryParse((table.Rows[index + i][column] as String), out digl);
                                bool bNeedProc = true;
                                foreach (SearchingColumnRows cr in this.lstColR)
                                {
                                    if (cr.IdxRow == index)
                                    {
                                        bNeedProc = false;
                                    }
                                }
                                if (bNeedProc)
                                {
                                    SearchingColumnRows colr = new SearchingColumnRows() { Column = column, Row = row, IdxRow = index, Sign = "Штрихкод" };
                                    qHeaderRow.Enqueue(colr);
                                    this.lstColR.Add(colr);
                                }
                                break;
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                    catch(IndexOutOfRangeException ex)
                    {
                        break;
                    }

                        i++;
                }
              /*  if (Array.Exists(data, this.HeaderNamesExists))
                {
                    qHeaderRow.Enqueue(new SearchingColumnRows() {  Column = column, Row = row});
                }*/
            }
            
        }
        private void GetRangeInvoice(object[] items, int index,  DataTable table, ref Hashtable htRange)
        {
            int probstart = -1;
            for (int i = 0; i <= items.Length - 1; i++)
            {
                if ((items[i] as String) == null)
                {
                    if ((items[i + 1] as String) != null)
                    {
                        String pattern = @"(\b)((кол((\w)+|(\s)*|(\W)+)во)|(к(\w)льк(\w)сть)|(к(\s)*|(\W)+(с){0,1}ть))(\b)";
                        Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
                        String pattern1 = @"(^)(\b)[№N]|(п(\W){0,1}п)(\b)($)";
                        Regex reg1 = new Regex(pattern, RegexOptions.IgnoreCase);
                       
                        String ritem = items[i + 1] as String;
                        if (reg.IsMatch(ritem) || reg.IsMatch(pattern1))
                            continue;
                        object[] items1 = table.Rows[index + 1].ItemArray;
                        if ((items1[i] as String) != null)
                        {
                            try
                            {
                                String item = items1[i] as String;
                                int result;
                                if (int.TryParse(item, out result))
                                {
                                    probstart = i;

                                }
                            }
                            catch(Exception e)
                            {
                            
                            }
                        }
                    }
                }
                else
                {
                    int startcolrange;
                    String item = items[i] as String;
                    this.GetIndicesRangeBorder(item, i, out startcolrange);
                    if (startcolrange == -1 && probstart == -1)
                        continue;
                    if (startcolrange == -1 && probstart >= 0)
                        startcolrange = probstart;
                    htRange["colstart"] = table.Columns[startcolrange];
                }
            }
        }
        private void GetIndicesRangeBorder(String item, int curritem, out int startcolrange)
        {
            startcolrange = -1;
           

            String pattern = @"(^)(\b)[№N]|(п(\W){0,1}п)(\b)($)";
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
            if (reg.IsMatch(item))
            {
                startcolrange = curritem;
            }
            

        }
        private int GetCountRows(DataRow header, DataTable table)
        {
           int count = 0;
           int index = table.Rows.IndexOf(header);
           object[] arr = header.ItemArray;
           for (int i = 0; i < arr.Length - 1; i++ )
           {
               if (arr[i] is String)
               { 
                  
               }
           }

           return count;
           
        }
        private bool HeaderNamesExists(object obj)
        {
            if (obj is String)
            {
                String pattern = @"(\b)((кол((\w)+|(\s)*|(\W)+)во)|(к(\w)льк(\w)сть)|(к(\s)*|(\W)+(с){0,1}ть))(\b)";;
                Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);  
               
                if (reg.IsMatch(obj as String))
                {
                    return true;
                }
            }

            return false;
        }

        private void AddColumns(object[] rowitems, int index, ref DataTable oResultDataTable)
        {
            int i = 0;
            while(i <=index)
            {
              if(rowitems[i] != null)  
                oResultDataTable.Columns.Add(new DataColumn(rowitems[i] as String));

              i++;
                
            }
        }

        private void AddRows(int index, DataTable table, ref DataTable oResultDataTable)
        {
            for (int i = index + 1; i <= table.Rows.Count - 1; i++)
            {
                object[] items = table.Rows[i].ItemArray;
                oResultDataTable.Rows.Add(items);
            }
                       
        }

        private void RemoveEmptyColumns(ref DataTable oResultDataTable)
        {
            List<DataColumn> lstDCol = new List<DataColumn>();

            foreach (DataColumn column in oResultDataTable.Columns)
            {
                String pattern = "Column(\\d+)";
                Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
                if (reg.IsMatch(column.ColumnName))
                {
                    lstDCol.Add(column);
                }

            }
            foreach (DataColumn column in lstDCol)
            {
                oResultDataTable.Columns.Remove(column);
            }
        }
        private List<object> GetDataColumns(String pattern, DataTable table, String price = null)
        {
            List<object> data = new List<object>();

            
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);

            if (price == null)
            {
                foreach (DataColumn column in table.Columns)
                {
                    if (reg.IsMatch(column.ColumnName))
                    {
                        int i = 0;
                        foreach (DataRow row in table.Rows)
                        {
                            data.Add(row[column]);
                        }

                        break;
                    }
                }
            }
            else if(price.Equals("price"))
            {
                List<DataColumn> priceColArr = new List<DataColumn>();
               
                foreach (DataColumn column in table.Columns)
                {
                    if (reg.IsMatch(column.ColumnName))
                    {
                        priceColArr.Add(column);
                    }
                }

                if (priceColArr.Count == 1)
                {
                    data.Add(priceColArr[0].ColumnName);
                    int j = 1;
                    foreach (DataRow row in table.Rows)
                    {
                        data.Add(row[priceColArr[0]]);
                    }
                }
                else
                {
                    try
                    {
                        String s = null;
                        String s1 = null;
                        int i = 0;
                       while(s == null && s1 == null)
                       {
                           s = table.Rows[i][priceColArr[0]] as String;
                           s1 = table.Rows[i][priceColArr[1]] as String;
                           i++;
                       }
                        float d = Convert.ToSingle(s);
                        float d1 = Convert.ToSingle(s1);

                        if (d > d1)
                        {
                            data.Add(priceColArr[0].ColumnName);
                            int j = 1;
                            foreach (DataRow row in table.Rows)
                            {
                                data.Add(row[priceColArr[0]]);
                            }
                        }
                        else if (d < d1)
                        {
                            data.Add(priceColArr[1].ColumnName);
                            int j = 1;
                            foreach (DataRow row in table.Rows)
                            {
                                data.Add(row[priceColArr[1]]) ;
                            }
                        }

                    }
                    catch(Exception e)
                    {
                        MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            return data;
        }

    }
}
