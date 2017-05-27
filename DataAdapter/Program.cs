using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using System.Windows.Forms;


namespace DataAdapter
{
    class Program
    {
        static void Main(string[] args)
        {
            
            String sPath = String.Empty;
            bool bIsPath = true;
            try
            {
                sPath = args[0];
               // MessageBox.Show(sPath);
               // sPath = @"C:\Users\ACER\Documents\testmail";
               // sPath = @"C:\Users\ACER\Documents\testmail\240417\";
            }
            catch (IndexOutOfRangeException e)
            {
                MessageBox.Show(e.Message, "Не передан параметр", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bIsPath = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bIsPath = false;
            }

            if(bIsPath)
            {
                using (NamedPipeClientStream oClientStream = new NamedPipeClientStream("DataAdapterChanel"))
               {
                   try
                   {
                           oClientStream.Connect();
                           StreamWriter oWriter = new StreamWriter(oClientStream);
                           oWriter.WriteLine(sPath);
                           oWriter.Flush();
                      
                       
                   }
                   catch(Exception e)
                   {
                       MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error); 
                   }
               }
            }
 
        }
    }
}
