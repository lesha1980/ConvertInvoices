using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media;
using System.Windows;
using System.Threading;

namespace ConverterToParamTXT
{
    public class LogInf
    {
        private String sLog;
        private String sLogLogInf;
       // private static Mutex _mutex = new Mutex();

        public LogInf()
        {
            this.sLog = "logconvert.txt";
            this.sLogLogInf = "logloginfconvert.txt";


        }
        private void logLogInf(string data)
        {
            using (StreamWriter sw = new StreamWriter(this.sLogLogInf, true))
            {
                sw.WriteLine(data);
            }
        }
     /*   public void logDataTrace(string data)
        {
            try
            {

                using (StreamWriter sw = new StreamWriter(this.sLog, true))
                {
                    sw.WriteLine(data);
                }

            }
            catch (DirectoryNotFoundException e)
            {
               // MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
               // this.logLogInf(e.Message);
               // this.logLogInf(e.StackTrace);
            }
            catch (FileLoadException e)
            {
               // MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
              //  this.logLogInf(e.Message);
               // this.logLogInf(e.StackTrace);

            }
            catch (PathTooLongException e)
            {
               // MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
               //  this.logLogInf(e.Message);
              //  this.logLogInf(e.StackTrace);

            }
            catch (IOException e)
            {
               // MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
              //  this.logLogInf(e.Message);
               // this.logLogInf(e.StackTrace);

            }
        }*/
        ~LogInf()
        {
            //_mutex.Dispose();
        }
        public void logDataError(String data, String trace)
        {
            Mutex _mutex = new Mutex();
            try
            {
                _mutex.WaitOne();
                if (File.Exists(this.sLog))
                {
                    FileInfo inf = new FileInfo(this.sLog);
                    long size = inf.Length;
                    if (size >= 1048576)
                        inf.Delete();
                }
                using (StreamWriter sw = new StreamWriter(this.sLog, true))
                {
                    sw.WriteLine(DateTime.Now.ToString());
                    sw.WriteLine(data);
                    sw.WriteLine(trace);
                }

            }
            catch (DirectoryNotFoundException e)
            {
                //MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                // this.logLogInf(e.Message);
                // this.logLogInf(e.StackTrace);

            }
            catch (FileLoadException e)
            {
                // MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                // this.logLogInf(e.Message);
                // this.logLogInf(e.StackTrace);

            }
            catch (PathTooLongException e)
            {
                //MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                // this.logLogInf(e.Message);
                // this.logLogInf(e.StackTrace);

            }
            catch (IOException e)
            {
                //MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                //  this.logLogInf(e.Message);
                // this.logLogInf(e.StackTrace);

            }
            finally {
                _mutex.ReleaseMutex();
                _mutex.Dispose();
            }
        }
    }
}
