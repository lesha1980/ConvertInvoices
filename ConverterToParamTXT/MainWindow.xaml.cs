using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using ComAdapter;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using SharpCompress.Reader.Rar;
using SharpCompress.Reader.Zip;
using SharpCompress.Reader;
using SharpCompress.Archive;
using System.Drawing;
using System.Windows.Resources;
using System.Reflection;
using Microsoft.Win32;
using System.Threading;
using System.Text.RegularExpressions;
//using SharpCompress.Common;


namespace ConverterToParamTXT
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate List<ParamTXTFile> FileBuilderAsync();
        private delegate void ParamTXTWriteAsync(ParamTXTFile paramTXT);
       // private FileBuilder fBuilder;
        private String sPath = String.Empty;           //путь к файлу
        private MonitorDataAdapterProcess oMonitor;
        private List<ParamTXTFile> lstParamTXT;
        private Hashtable htPerformParamTXT;
        private String pattern = @"(^)(\.xls[xm]{0,1})($)";
        private ObservableCollection<FileProcessedItem> pathFile;
        private LogInf oLog;
        private String SerializeFileName = "objects.dat";
        private FileInfo SerializeFileInfo;
        private System.Windows.Forms.NotifyIcon _notif;
        private String AppNameKey = "ConverterTXTParam";
        private String RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private bool _loaded = false;
        private DirectoryInfo _dirinf;
        private static Mutex oMutex = new Mutex();

        public MainWindow()
        {
            InitializeComponent();
            this.Closed += (sender, e) => this.Dispatcher.InvokeShutdown();
            this.SerializeFileInfo = new FileInfo(this.SerializeFileName);
            this.oMonitor = MonitorDataAdapterProcess.CreateMonitorDA();//new MonitorDataAdapterProcess();
            this.oMonitor.oMonitorEvent += MonitorAdapterEventHadler;
            this.oLog = new LogInf();
            this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            this.htPerformParamTXT = new Hashtable();
          try { 
                  if(!File.Exists(this.SerializeFileName))
                        throw new FileNotFoundException("Файл данных " +this.SerializeFileName+ " не обнаружен");

                  this.DeserializeObject();
              }
              catch(FileNotFoundException e)
              {
                this.oLog.logDataError(e.Message, e.StackTrace);
               
                this.pathFile = new ObservableCollection<FileProcessedItem>();
              }
              catch(IOException e)
              {
              this.oLog.logDataError(e.Message, e.StackTrace);
              }
          this.listProcedFiles.ItemsSource = this.pathFile;
          //  this.pathFile = new ObservableCollection<FileProcessedItem>();
          //  this.oLog = new LogInf();
          this._notif = new System.Windows.Forms.NotifyIcon();
          StreamResourceInfo _streaminf = Application.GetResourceStream(new Uri("Resources/soundconverter.ico", UriKind.Relative));
          this._notif.Icon = new Icon(_streaminf.Stream);
          this._notif.Visible = true;
          this._notif.Text = "Конвертер Накладных";
          System.Windows.Forms.MenuItem item2 = new System.Windows.Forms.MenuItem("Выход");
          item2.Click += delegate(object sender, EventArgs e) {
              ApplicationCommands.Close.Execute(null, exitBut);
          };
          System.Windows.Forms.MenuItem item3 = new System.Windows.Forms.MenuItem("Очистить список");
          item3.Click += delegate(object sender, EventArgs e)
          {
              ConverterCommands.ClearList.Execute(null, clearListBut);
          };

          System.Windows.Forms.MenuItem item1 = new System.Windows.Forms.MenuItem("Очистить текстовое поле");
          item1.Click += delegate(object sender, EventArgs e)
          {
              ConverterCommands.ClearText.Execute(null, this.clearTextBut);
          };
          System.Windows.Forms.MenuItem item4 = new System.Windows.Forms.MenuItem("Очистить журнал");
          item4.Click += delegate(object sender, EventArgs e)
          {
              ConverterCommands.ClearLog.Execute(null, this.clearLogBut);
          };
          System.Windows.Forms.MenuItem item5 = new System.Windows.Forms.MenuItem("Развернуть окно");
          item5.Click += delegate(object sender, EventArgs e)
          {
              this.Show();
              this.WindowState = System.Windows.WindowState.Normal;
          };
          this._notif.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[]{item5, item1, item3, item4, item2});
          this._notif.DoubleClick += delegate(object sender, EventArgs e) 
          {
              this.Show();
              this.WindowState = System.Windows.WindowState.Normal;
          };
          this._notif.MouseClick += delegate(object sender, System.Windows.Forms.MouseEventArgs e)
          {
               if(e.Button == System.Windows.Forms.MouseButtons.Left)
               {
                   MethodInfo inf = typeof(System.Windows.Forms.NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                   inf.Invoke(this._notif, null);
               }
          };
        }
        public ObservableCollection<FileProcessedItem> FilePath {
            get {
                 return  this.pathFile;
            }
        }
        private void SerializeObjects()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using(FileStream fs = new FileStream(this.SerializeFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                bf.Serialize(fs, this.pathFile);
            }
           
        }
        private void DeserializeObject()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (Stream fs = File.OpenRead(this.SerializeFileName))
            {
                this.pathFile = (ObservableCollection<FileProcessedItem>)bf.Deserialize(fs);
            }
        }

        private List<String> getListFilesTargetDirectory(String path)
        {
            List<String> lstFilePaths = new List<string>();
            String sTargetPath = this.getTargetDirectory(path);
            string[] finfs = null;
            Regex reg = new Regex(this.pattern, RegexOptions.IgnoreCase);
            try
            {
                finfs = Directory.GetFiles(sTargetPath);
            }
            catch(Exception e)
            {
                this.oLog.logDataError(e.Message, e.StackTrace);
            }

            foreach(String s in finfs)
            {
                FileInfo finf = new FileInfo(s);
                if (finf.Extension.Equals(".rar"))
                { 
                    using(Stream stream = File.OpenRead(s))
                    {
                        var reader = ReaderFactory.Open(stream);
                        while(reader.MoveToNextEntry())
                        {
                           if(!reader.Entry.IsDirectory)
                           {
                               try
                               {
                                   reader.WriteEntryToDirectory(sTargetPath, SharpCompress.Common.ExtractOptions.ExtractFullPath);
                                   FileInfo finf1 = new FileInfo(sTargetPath + @"\" + reader.Entry.Key);
                                   if (finf1.Extension.Equals(".xls") || finf1.Extension.Equals(".xlsx") || finf1.Extension.Equals(".xlsm") || finf1.Extension.Equals(".txt"))
                                        lstFilePaths.Add(sTargetPath + @"\" + reader.Entry.Key);
                               }
                               catch(Exception e)
                               {
                                   this.oLog.logDataError(e.Message, e.StackTrace);
                               }
                           }
                        }
                    }
                }
                if (finf.Extension.Equals(".zip"))
                {
                    var archive = ArchiveFactory.Open(finf);
                    foreach(var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            try
                            {
                                entry.WriteToDirectory(sTargetPath, SharpCompress.Common.ExtractOptions.ExtractFullPath);
                                FileInfo finf1 = new FileInfo(sTargetPath + @"\" + entry.Key);
                                if (reg.IsMatch(finf1.Extension) || finf1.Extension.Equals(".txt"))
                                  lstFilePaths.Add(sTargetPath + @"\" + entry.Key);
                            }
                            catch(Exception e)
                            {
                                this.oLog.logDataError(e.Message, e.StackTrace);
                            }
                        }
                    }
                }
                if (reg.IsMatch(finf.Extension)|| finf.Extension.Equals(".txt"))
                    lstFilePaths.Add(finf.FullName);
                
            }

            return lstFilePaths;
        }

     

        private String getTargetDirectory(String path)
        {
            DirectoryInfo drinf = new DirectoryInfo(path);
            DirectoryInfo[] drinfs = drinf.GetDirectories();
            String sTargetPath = String.Empty;
            foreach(DirectoryInfo d in drinfs)
            {
               // String date = DateTime.Now.Date.ToString("ddMMyy"); 
                if (d.CreationTime.Date == DateTime.Now.Date)
                {
                    sTargetPath = d.FullName;
                }
            }
            this._dirinf = this.CreateSubDir(sTargetPath);
            return sTargetPath;
            
        }
        private DirectoryInfo CreateSubDir(String _dir)
        {
            String _name = "subdir";
            DirectoryInfo _dirinf = Directory.CreateDirectory(_dir + "\\" + _name);
            return _dirinf;
        }
      
        private void MonitorAdapterEventHadler(object sender, MonitorDataAdapterEventArgs e)
        {
            try
            {
                String sPath = e.Data;
                
                if (sPath == null || sPath.Equals(String.Empty))
                    throw new MonitorAdapterException("DataAdapter нет имени файла или нет ссылки на файл");

                if (sPath.LastIndexOf('\n') != -1)
                {
                    sPath = sPath.Substring(0, sPath.Length - 1);

                }
                if (sPath.LastIndexOf('\r') != -1)
                {
                    sPath = sPath.Substring(0, sPath.Length - 1);
                }
           
            List<String> lstFilePaths = this.getListFilesTargetDirectory(sPath);
               // List<String> lstFilePaths = new List<string>();
               // lstFilePaths.Add(@"C:\Users\ACER\Documents\testmail\240417\престиж(2).xls");
                // MessageBox.Show(e.Data, "Title", MessageBoxButton.OK, MessageBoxImage.Information);
            foreach(String s in lstFilePaths)
            {
                FileBuilder fBuilder = new ComAdapter.ComAdapter();
                fBuilder.ParentPath = this._dirinf;
                fBuilder.Initialize(s);
                
                FileBuilderAsync fileBuilder = fBuilder.Start;
                fileBuilder.BeginInvoke(EndBuild, fileBuilder);
            }
            }
            catch(MonitorAdapterException ex)
            {
                this.oLog.logDataError(ex.Message, ex.StackTrace);
            }
            catch(Exception ex)
            {
                this.oLog.logDataError(ex.Message, ex.StackTrace);

            }

        }

     /*   private void Button_Click_1(object sender, RoutedEventArgs e)
        {
          
            // MessageBox.Show(e.Data, "Title", MessageBoxButton.OK, MessageBoxImage.Information);
          //  this.fBuilder = new ComAdapter.ComAdapter();
          //  this.fBuilder.Initialize(@"C:\Users\ACER\Documents\testmail\testmail.zip");
            //FileBuilderAsync fileBuilder = this.fBuilder.Start;
            //fileBuilder.BeginInvoke(EndBuild, fileBuilder);

      }*/

        ~MainWindow()
        {
            oMutex.Dispose();
        }
        private void EndBuild(IAsyncResult ar)
        {
            try
            {
                oMutex.WaitOne(); 
                FileBuilderAsync builder = (FileBuilderAsync)ar.AsyncState;
                // builder.EndInvoke(ar);
                // this.lstParamTXT = this.fBuilder.Result();
                this.lstParamTXT = builder.EndInvoke(ar);
                foreach (ParamTXTFile fparam in this.lstParamTXT)
                {
                    
                   if(!File.Exists(fparam.FileName))
                   {
                    StreamWriter sw = null;
                    FileProcessedItem item = new FileProcessedItem();
                    try
                    {
                        if (fparam == null)
                            throw new ParamTXTException("Ссылка не указывает на ParamTXTFile");
                        String m = String.Empty;
                        int count = fparam.AmountProducts.Count - 1;
                        FileInfo finf = new FileInfo(fparam.FileName);
                        if (finf.Extension.Equals(".txt"))
                        {
                            sw = new StreamWriter(fparam.FileName, true);
                            sw.WriteLine("@@@@");
                            sw.Close();
                            sw.Dispose();
                        }


                        for (int i = 0; i <= count; i++)
                        {
                            String s = this.CreatStrWrite(fparam);
                            sw = new StreamWriter(fparam.FileName, true);
                            sw.WriteLine(s);
                            sw.Close();
                            sw.Dispose();

                        }

                        item.DateProcessed = DateTime.Now.ToString();
                        item.NameFile = fparam.FileName;
                        item.Status = true;


                    }
                    catch (ParamTXTException e)
                    {
                        this.oLog.logDataError(e.Message, e.StackTrace);
                        item.DateProcessed = DateTime.Now.ToString();
                        item.NameFile = fparam.FileSource;
                        item.Status = false;

                    }
                    catch (DirectoryNotFoundException e)
                    {
                        MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                        this.oLog.logDataError(e.Message, e.StackTrace);
                        item.DateProcessed = DateTime.Now.ToString();
                        item.NameFile = fparam.FileSource;
                        item.Status = false;

                    }
                    catch (FileLoadException e)
                    {
                        MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                        this.oLog.logDataError(e.Message, e.StackTrace);
                        item.DateProcessed = DateTime.Now.ToString();
                        item.NameFile = fparam.FileSource;
                        item.Status = false;

                    }
                    catch (PathTooLongException e)
                    {
                        MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                        this.oLog.logDataError(e.Message, e.StackTrace);
                        item.DateProcessed = DateTime.Now.ToString();
                        item.NameFile = fparam.FileSource;
                        item.Status = false;

                    }
                    catch (IOException e)
                    {
                        MessageBox.Show(e.Message, e.HResult.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                        this.oLog.logDataError(e.Message, e.StackTrace);
                        item.DateProcessed = DateTime.Now.ToString();
                        item.NameFile = fparam.FileSource;
                        item.Status = false;

                    }
                    finally
                    {
                        if (sw != null)
                        {
                            sw.Close();
                            sw.Dispose();
                        }

                    }
                    if (this.pathFile.Count >= 10)
                    {
                        this.Dispatcher.Invoke(() => this.pathFile.RemoveAt(0));

                    }
                    this.Dispatcher.Invoke(delegate { 
                        this.pathFile.Add(item);
                        this.listProcedFiles.ItemsSource = this.pathFile;
                    });
                    
                }
                 
                }
                oMutex.ReleaseMutex();
            }
            catch(NullReferenceException e)
            {
                this.oLog.logDataError("Файл не обработан: " + e.Message, e.StackTrace);
              
            }
            catch(Exception e)
            {
                this.oLog.logDataError(e.Message, e.StackTrace);
            }
        }

        private String CreatStrWrite(ParamTXTFile fparam)
        { 
            String resultstr = String.Empty;
            try
            {
                if(fparam.BarCode.Count != 0)
                {
                if (fparam.BarCode.Peek() == null)
                {
                    fparam.BarCode.Pop();
                    resultstr += "";
                }
                else
                {
                    String s = fparam.BarCode.Pop();
                    if (s.LastIndexOf(',') != -1)
                    {
                        s = s.Substring(0, s.LastIndexOf(','));
                    }
                    resultstr += s;
                }
               }
                else
                    throw new ParamTXTException("Стек BarCode пуст: елемент " + fparam.FileSource);
            }
            catch (ParamTXTException e)
            {
                this.oLog.logDataError(e.Message, e.StackTrace);
            }
            catch (Exception e)
            {
                this.oLog.logDataError(e.Message, e.StackTrace);
            }
            

                resultstr += fparam.Separator;
        try{    
              if(fparam.NameProduct.Count != 0)
              {
                if (fparam.NameProduct.Peek() == null)
                {
                    fparam.NameProduct.Pop();
                    resultstr += "";
                }
                else
                    resultstr += fparam.NameProduct.Pop();
              }
              else
                   throw new ParamTXTException("Стек NameProduct пуст: елемент " + fparam.FileSource);
            }
            catch (ParamTXTException e)
            {
                this.oLog.logDataError(e.Message, e.StackTrace);
            }
            catch (Exception e)
            {
                this.oLog.logDataError(e.Message, e.StackTrace);
            }
                
                resultstr += fparam.Separator;
        try{
              if(fparam.AmountProducts.Count != 0)
              {
                if (fparam.AmountProducts.Peek() == null)
                {
                    fparam.AmountProducts.Pop();
                    resultstr += "";
                }
                else
                    resultstr += fparam.AmountProducts.Pop();
              }
              else
                  throw new ParamTXTException("Стек AmountProducts пуст: елемент " + fparam.FileSource);
            }
            catch (ParamTXTException e)
            {
                this.oLog.logDataError(e.Message, e.StackTrace);
            }
            catch (Exception e)
            {
                this.oLog.logDataError(e.Message, e.StackTrace);
            }

                resultstr += fparam.Separator;
        try{ 
                if (fparam.PricePDV.Count > 0)
                {
                    if (fparam.PricePDV.Peek() == null)
                    {
                        fparam.PricePDV.Pop();
                        resultstr += "";
                    }
                    else
                        resultstr += fparam.PricePDV.Pop();
                }
                else
                  throw new ParamTXTException("Стек PricePDV пуст: елемент " + fparam.FileSource);
                resultstr += fparam.Separator;
            }
            catch (ParamTXTException e)
            {
                this.oLog.logDataError(e.Message, e.StackTrace);
            }
            catch (Exception e)
            {
                this.oLog.logDataError(e.Message, e.StackTrace);
            }

                
         try{ 
                if (fparam.Price.Count > 0)
                {
                    if (fparam.Price.Peek() == null)
                    {
                        fparam.Price.Pop();
                        resultstr += "";
                    }
                    else
                        resultstr += fparam.Price.Pop();

                    resultstr += fparam.Separator;
                }
                else
                    throw new ParamTXTException("Стек Price пуст: елемент " + fparam.FileSource);
            }
            catch(ParamTXTException e)
            {
                this.oLog.logDataError(e.Message, e.StackTrace);
            }
            catch(Exception e)
            {
                this.oLog.logDataError(e.Message, e.StackTrace);
            }

         

            return resultstr;





        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SerializeObjects();
            
        }

        private void Button_Click_Exit(object sender, ExecutedRoutedEventArgs e)
        {
           // int a = Application.Current.Windows.Count;
            Application.Current.MainWindow.Close();
           // Process.GetCurrentProcess().Kill();
        }

        private void listProcedFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject obj = (DependencyObject)e.OriginalSource;

            while (obj != null && obj != this.listProcedFiles)
            {
                if (obj.GetType() == typeof(ListViewItem))
                {
                    try
                    {
                        // Do something here
                        String file = ((obj as ListViewItem).Content as FileProcessedItem).NameFile;
                        FileInfo fInf = new FileInfo(file);

                        if (fInf.Extension.Equals(".txt"))
                        {
                            using (Stream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None))
                            {
                                using (StreamReader sr = new StreamReader(fs))
                                {
                                    String data = sr.ReadToEnd();
                                    this.textFile.Text = data;
                                }
                            }
                        }
                        else if (fInf.Extension.Equals(".xls") || fInf.Extension.Equals(".xlsx") || fInf.Extension.Equals("xlsm"))
                        {
                            Process.Start(file);
                        }
                        break;
                    }
                    catch(Exception ex)
                    {
                        this.oLog.logDataError(ex.Message, ex.StackTrace);
                    }
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            
            this.oMonitor.Dispose();
        
            base.OnClosed(e);
            this._notif.Visible = false;
            Application.Current.Shutdown(0);
           // Process.GetCurrentProcess().Kill();
            
        }

        private void ClearListExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.pathFile.Clear();
            try
            {
                if (File.Exists(this.SerializeFileName))
                {
                    File.Delete(this.SerializeFileName);
                }
            }
            catch(FileNotFoundException ex)
            {
                this.oLog.logDataError("Файл " + this.SerializeFileName + " не найден" + ex.Message, ex.StackTrace);
            }
            catch(IOException ex)
            {
                this.oLog.logDataError(ex.Message, ex.StackTrace);     
            }

        }

        private void ClearTextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.textFile.Text = "";
        }

        private void ClearLogExecuted(object sender, ExecutedRoutedEventArgs e)
        {
             if(File.Exists("logconvert.txt"))
             {
                 File.Delete("logconvert.txt");
             }
        }

        protected override void OnStateChanged (EventArgs e)
        {
           
            if (this.WindowState == System.Windows.WindowState.Minimized)
                this.Hide();
                
            base.OnStateChanged(e);
        }

        private void MinToTrayExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void AddAutorunExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                string _curuser = Registry.CurrentUser.Name + "\\" + this.RunKey;
                string _key = (string)Registry.GetValue(_curuser, this.AppNameKey, String.Empty);
                if (_key.Equals(String.Empty))
                {
                    string _exeapp = "\"" + System.Windows.Forms.Application.StartupPath + @"\ConverterToParamTXT.exe" +"\"";
                    Registry.SetValue(_curuser, this.AppNameKey, _exeapp);
                }
            }
            catch(Exception ex)
            {
                this.oLog.logDataError(ex.Message, ex.StackTrace);
            }
        }

        private void DelAutorunExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RegistryKey _regkey = null;
            try
            {
                string _curuser = Registry.CurrentUser.Name + "\\" + this.RunKey;
                string _key = (string)Registry.GetValue(_curuser, this.AppNameKey, String.Empty);
                if (!_key.Equals(String.Empty))
                {
                    _regkey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                    _regkey.DeleteValue(this.AppNameKey);
                }
            }
            catch (Exception ex)
            {
                this.oLog.logDataError(ex.Message, ex.StackTrace);
            }
            finally
            {
                if (_regkey != null)
                    _regkey.Close();
            }
        }

        private void Grid_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            if (this._loaded)
            {
                Grid _grid = sender as Grid;
                RowDefinition rd = _grid.RowDefinitions[1];

                ListView _child = _grid.Children[1] as ListView;
                ScrollViewer _tb = _grid.Children[2] as ScrollViewer;

                if (e.WidthChanged)
                {
                    _child.Width = e.NewSize.Width;
                }
                if (e.HeightChanged)
                {
                    _tb.Height = e.NewSize.Height;
                }
            }
            else
                this._loaded = true;
        }

        private void TextBox_TextInput_1(object sender, TextCompositionEventArgs e)
        {

        }

        
       
       
    }
}
