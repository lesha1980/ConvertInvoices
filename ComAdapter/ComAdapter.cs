using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComTXT;
using ConverterToParamTXT;
using ComXLS;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;

namespace ComAdapter
{
    public class ComAdapter: FileBuilder
    {
        private String pattern = @"(^)(xls[xm]{0,1})($)";
       // private XLSFileBuilder oFileBuilder;
       // private TXTFileBuilder oTXTFileBuilder;
        private FileBuilder oFileBuilder;
       // private static Mutex oMutex = new Mutex();
        private String getExtension(string sPath)
        {
            String sExt = string.Empty;
            try
            {
                FileInfo oInfoFile = new FileInfo(sPath);
                sExt = oInfoFile.Extension;
                sExt = sExt.Substring(1);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
            return sExt; 

        }
        public override void Initialize(string sPath)
        {
            //MessageBox.Show("Мы перед", "Мы перед", MessageBoxButtons.OK, MessageBoxIcon.Information);
            String sExt = this.getExtension(sPath);
            Regex reg = new Regex(this.pattern, RegexOptions.IgnoreCase);
            if (reg.IsMatch(sExt))
            {
                try
                {
                   // MessageBox.Show("Мы тут", "Мы тут", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //this.oFile = new XLSProcessed(sPath);
                    this.oFileBuilder = new XLSFileBuilder();
                    this.oFileBuilder.ParentPath = this.ParentPath;
                    this.oFileBuilder.Initialize(sPath);
                }
                catch(Exception e)
                {
                   MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if(sExt.Equals("txt") || sExt.Equals("csv"))
            {
                if (sExt.Equals("csv"))
                {
                    String s = sPath.Substring(0, sPath.IndexOf('.'));
                    s += ".txt";
                }
                try
                {
                  // MessageBox.Show("Мы тут txt", "Мы тут", MessageBoxButtons.OK, MessageBoxIcon.Information);
                   // this.oFile = new TXTProcessed(sPath);
                    this.oFileBuilder = new TXTFileBuilder();
                    this.oFileBuilder.ParentPath = this.ParentPath;
                    this.oFileBuilder.Initialize(sPath);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
          
           // base.Initialize(this.oFile);
        }

        public override List<ParamTXTFile> Start()
        {
            Mutex oMutex = new Mutex();
            try
            {
                oMutex.WaitOne();
                List<ParamTXTFile> ptxtlst = this.oFileBuilder.Start();
                oMutex.ReleaseMutex();
                return ptxtlst;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally {
                oMutex.Dispose();
            }
           // this.lstParamTXTFile = this.oFileBuilder.Result();
            //base.Start();
        }
        ~ComAdapter()
        {
            //oMutex.Dispose();
        }
        public override void Finish()
        {
        
        }
    }
}
