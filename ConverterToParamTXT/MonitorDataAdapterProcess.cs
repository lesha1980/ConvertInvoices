using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using System.Windows;
//using System.Windows.Forms;


namespace ConverterToParamTXT
{
    public class MonitorDataAdapterProcess: IDisposable
    {
        public delegate void MonitorDataAdapterHandler(object sender, MonitorDataAdapterEventArgs args);
        private String sBuffer = String.Empty;
        private Thread oThread;
        private ThreadStart oThreadStart;
        public event MonitorDataAdapterHandler oMonitorEvent;
        private LogInf oLog;
        private NamedPipeServerStream oServerPipeStream;
        private static MonitorDataAdapterProcess oMonitor;
        private TimeSpan nDelay;


        private MonitorDataAdapterProcess()
        {
            this.oLog = new LogInf();
            this.oThreadStart = new ThreadStart(this.StartServer);
            this.oThread = new Thread(this.oThreadStart);
            this.oThread.Start();
        }

        public static MonitorDataAdapterProcess CreateMonitorDA()
        { 

              if(oMonitor == null)
              {
                  oMonitor = new MonitorDataAdapterProcess();
              }
              
              return oMonitor;
        }

        public TimeSpan Delay {
            get {
                return this.nDelay;
            }
            set {
                this.nDelay = value;
            }
        }
        public String Buffer {
            get {
                return this.sBuffer;
            }
        }
        public void StartServer()
        {
            while (true)
            {
                try
                {

                    using (oServerPipeStream = new NamedPipeServerStream("DataAdapterChanel"))
                    {
                        oServerPipeStream.WaitForConnection();
                        using (StreamReader oReader = new StreamReader(oServerPipeStream))
                        {
                            this.sBuffer = oReader.ReadToEnd();
                           
                            
                           // MessageBox.Show(this.sBuffer);
                            if (this.sBuffer == null || this.sBuffer.Equals(String.Empty))
                                throw new MonitorAdapterException("В буфер не переданы данные пути к файлу MonitorAdapterException");
                            // MessageBox.Show(this.sBuffer, "Title", MessageBoxButton.OK, MessageBoxImage.Information);
                          
                            if (this.oMonitorEvent != null)
                                this.oMonitorEvent(this, new MonitorDataAdapterEventArgs(this.sBuffer));
                        }

                   }
                }
                catch(MonitorAdapterException e)
                {
                    this.oLog.logDataError(e.Message, e.StackTrace);

                }
                catch(IOException e)
                {
                    this.oLog.logDataError(e.Message, e.StackTrace);

                }
                catch(Exception e)
                {
                    this.oLog.logDataError(e.Message, e.StackTrace);

                }
            }
        }
        ~MonitorDataAdapterProcess()
        {
            //this.oThread.IsBackground = true;
            //this.Dispose();
        }
      /*  public void StartThreadAnother()
        {
            if (!this.oThread.IsAlive)
            {
                this.oThread.Start();
                MessageBox.Show("Поток вновь запущен", "Title", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }*/

        public void Dispose()
        {
          if(this.oThread != null)
            this.oThread.IsBackground = true;
            if (this.oServerPipeStream != null)
            {
                if (this.oServerPipeStream.IsConnected)
                    this.oServerPipeStream.Disconnect();
              
                this.oServerPipeStream.Close();
                this.oServerPipeStream = null;
            /* if(this.oThread.IsAlive)
             {
                this.oThread.Abort();
                this.oThread = null;
             }*/
            }
        }

       
    }

    public class MonitorDataAdapterEventArgs: EventArgs
    {
        private String sData = String.Empty;

        public MonitorDataAdapterEventArgs(String data)
        {
            this.sData = data;
        }

        public String Data
        {
            get {
                return this.sData;
            }
        }
    }
}
