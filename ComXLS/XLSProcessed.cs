using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ConverterToParamTXT;


namespace ComXLS
{
    public class XLSProcessed: ProcessedFile
    {
        List<Object> list = new List<object>();
        List<DataSet> lstDataSet = new List<DataSet>();
        private LogInf oLog;
            

        public XLSProcessed(String sPath): base(sPath)
        {
            this.ExcelConnection = new OleDbConnection();
            this.ExcelCommand = new OleDbCommand();
            this.oDataAdapter = new OleDbDataAdapter();
            String sConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+this.FileName+";Extended Properties=\"Excel 12.0;HDR=NO;IMEX=1;MAXSCANROWS=300;READONLY=FALSE\";";
            this.ExcelBuilder = new OleDbConnectionStringBuilder(sConnectionString);
            ExcelConnection.ConnectionString = ExcelBuilder.ConnectionString;
            this.oLog = new LogInf();
            
            
            
        }

        public List<DataSet> LstDataSet
        {
            get {
                return this.lstDataSet;
            }
        }
        

        private DataTable getXLSSchema()
        {
            return (ExcelConnection as OleDbConnection).GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
        }

        private void getNameSheets(DataTable oDataTable)
        {
            foreach (DataRow row in oDataTable.Rows)
            {
                list.Add(row["TABLE_NAME"]);
            }
            
        }

        public override void ReadFile()
        {
        try {

           
            ExcelConnection.Open();
                DataTable dtSheets = this.getXLSSchema();
                this.getNameSheets(dtSheets);
                for (int i = 0; i <= list.Count - 1; i++)
                {
                    ExcelCommand.CommandText = "select * from[" + list[i] + "]";
                    this.oDataAdapter.SelectCommand = ExcelCommand;
                    this.oDataAdapter.SelectCommand.Connection = this.ExcelConnection;
                    DataSet oDataSet = new DataSet();
                    this.oDataAdapter.Fill(oDataSet);
                    lstDataSet.Add(oDataSet);

                }
                
               
            }
            catch(Exception e)
            {
                this.oLog.logDataError(e.Message, e.StackTrace);

            }
            finally{

                ExcelConnection.Close();
            }
           
         
        }

        public override void ProcessedResult()
        {
            
        }
    }
}
