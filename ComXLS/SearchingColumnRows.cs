using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ConverterToParamTXT;

namespace ComXLS
{
    public class SearchingColumnRows
    {
        private String sTypeSign;
        private DataRow oRow;
        private DataColumn oColumn;
        private int idxRow;
        private bool bRowProc;

        public SearchingColumnRows()
        { 
        
        }
        public SearchingColumnRows(DataColumn column, DataRow row, String sign, int idxRow)
        {
            this.oColumn = column;
            this.oRow = row;
            this.sTypeSign = sign;
            this.idxRow = idxRow;
            this.bRowProc = true;

        }
        public bool RowProc {
            get {
                return this.bRowProc;
            }
            set {
                this.bRowProc = value;
            }
        }
        public int IdxRow {

            get {
                return this.idxRow;
            }
            set {
                this.idxRow = value;
            }
        }
        public DataRow Row
        {
            get {
                return this.oRow;
            }
            set {
                this.oRow = value;
            }
        }

        public DataColumn Column
        {
            get
            {
                return this.oColumn;
            }
            set
            {
                this.oColumn = value;
            }

        }
        public String Sign
        {
            get {
                return this.sTypeSign;
            }
            set {
                this.sTypeSign = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType()) return false;

            SearchingColumnRows colr = (SearchingColumnRows)obj;

            return (this.idxRow == colr.idxRow);
        }
    }
}
