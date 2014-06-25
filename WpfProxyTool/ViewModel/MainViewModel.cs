using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using WpfProxyTool.Model;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Data;
using System.IO;
using System.Net;
using System.Collections;

namespace WpfProxyTool.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<ProxyLeechListModel> LeechList { get; set; }
        public ObservableCollection<Proxy> ProxyList { get; set; }
        public LeecherModel LeechModel { get; set; }
        private static readonly object ListLock = new object();
        private List<ProxyLeechListModel> SelectedUrls;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            LeechList = new ObservableCollection<ProxyLeechListModel>();
            BindingOperations.EnableCollectionSynchronization(LeechList, ListLock);

            ProxyList = new ObservableCollection<Proxy>();
            BindingOperations.EnableCollectionSynchronization(ProxyList, ListLock);

            SelectedUrls = new List<ProxyLeechListModel>();

            LeechModel = new LeecherModel();
            LeechModel.LeechTimeout = "5";
            LeechModel.LeechStartButtonEnabled = true;
            LeechModel.ProgressBarEnabled = false;

            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }

        private RelayCommand _openLeechListCommand;
        public RelayCommand OpenLeechListCommand
        {
            get
            {
                {
                    if (_openLeechListCommand == null)
                        _openLeechListCommand = new RelayCommand(new Action(OpenLeechListExecuted));
                    return _openLeechListCommand;
                }
            }
        }

        private void OpenLeechListExecuted()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.InitialDirectory = @"C:\";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Task.Factory.StartNew(() => ReadLeechFile(System.IO.File.ReadAllText(ofd.FileName)));
                //MessageBox.Show(ofd.FileName);
            }
        }

        private void ReadLeechFile(String file)
        {
            // Clear the list before reading a new file
            LeechList.Clear();

            // http://regexlib.com/Search.aspx?k=url&AspxAutoDetectCookieSupport=1
            Regex regex = new Regex(@"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?");
            MatchCollection matches = regex.Matches(file);
            foreach (Match match in matches)
            {
                LeechList.Add(new ProxyLeechListModel
                    {
                        URL = match.ToString()
                    }
                );
            }
        }

        private RelayCommand<DragEventArgs> _dataGridLeecherDropCommand;
        public RelayCommand<DragEventArgs> DataGridLeecherDropCommand
        {
            get
            {
                return _dataGridLeecherDropCommand ?? (_dataGridLeecherDropCommand = new RelayCommand<DragEventArgs>(DataGridLeecherDrop));
            }
        }

        private void DataGridLeecherDrop(DragEventArgs e)
        {
            // http://stackoverflow.com/questions/11671803/get-source-of-a-dragdrop
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            Task.Factory.StartNew(() => ReadLeechFile(files[0]));
        }


        private RelayCommand _startLeechingCommand;
        public RelayCommand StartLeechingCommand
        {
            get
            {
                {
                    if (_startLeechingCommand == null)
                        _startLeechingCommand = new RelayCommand(new Action(StartLeechingExecuted));
                    return _startLeechingCommand;
                }
            }
        }

        private void StartLeechingExecuted()
        {
            LeechModel.LeechStartButtonEnabled = false;
            LeechModel.ProgressBarEnabled = true;

            //TaskForEach(5);
            ParallelForEach();

            //leechModel.ProgressBarEnabled = false;
            LeechModel.LeechStartButtonEnabled = true;
        }

        // Fasted method to query the data
        private void ParallelForEach()
        {
            // Solution? http://stackoverflow.com/questions/12337671/using-async-await-for-multiple-tasks
            Parallel.ForEach(LeechList, async item =>
            {
                WebContent source = new WebContent();
                source = await GetUrlContentAsync(item.URL);

                string content = System.Text.Encoding.Default.GetString(source.content);
                List<Proxy> proxys = GetIpsFromContent(content);
                foreach (var ip in proxys)
                {
                    ProxyList.Add(ip);
                }
                // item.Proxys.Add(content);
                item.Date = DateTime.Now;
                item.Count = proxys.Count;
                item.Reply = source.status;
            }
            );
        }

        private void TaskForEach(int taskCount)
        {
            // http://msdn.microsoft.com/de-de/library/dd270695%28v=vs.110%29.aspx
            // Problem with Async; Not Working as expected
            //for (int i = 0; i < leechList.Count; i+=taskCount - 1)
            for (int i = 0; i < 4; i += 5)
            {
                List<ProxyLeechListModel> items = new List<ProxyLeechListModel>();
                for (int t = i; t < i + taskCount; t++)
                {
                    items.Add(LeechList[t]);
                }

                Task[] tasks = new Task[4];
                for (int v = 0; v < taskCount - 1; v++)
                {
                    if (v == 5)
                        MessageBox.Show("Is 5.");
                    if (v < items.Count)
                    {
                        tasks[v] = new Task(() => MakeTaskRequest(LeechList.IndexOf(items[v])));
                    }
                }
                Task.WaitAll(tasks);
            }
        }

        private async void MakeTaskRequest(int index)
        {
            WebContent source = new WebContent();
            source = await GetUrlContentAsync(LeechList[index].URL);

            string content = System.Text.Encoding.Default.GetString(source.content);
            // LeechList[index].Proxys.Add(content);
            LeechList[index].Date = DateTime.Now;
            LeechList[index].Count = source.content.Length;
            LeechList[index].Reply = source.status;
        }

        private async Task<WebContent> GetUrlContentAsync(string url)
        {
            // http://msdn.microsoft.com/de-de/library/hh300224.aspx
            var result = new WebContent();
            try
            {
                var content = new MemoryStream();
                var webReq = (HttpWebRequest)WebRequest.Create(url);
                webReq.Timeout = Int32.Parse(LeechModel.LeechTimeout) * 1000;
                using (var response = await webReq.GetResponseAsync())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        await responseStream.CopyToAsync(content);
                        result.content = content.ToArray();
                        result.status = ((HttpWebResponse)response).StatusCode.ToString();
                    }
                }
            }
            catch (WebException ex)
            {
                result.content = new byte[0];
                if (ex.Response != null)
                {
                    var resp = (HttpWebResponse)ex.Response;
                    result.status = resp.StatusCode.ToString();
                }
                else
                {
                    result.status = "Not Found";
                }
            }
            return result;
        }

        private RelayCommand _clearLeechListCommand;
        public RelayCommand ClearLeechListCommand
        {
            get
            {
                {
                    if (_clearLeechListCommand == null)
                        _clearLeechListCommand = new RelayCommand(new Action(ClearLeechListExecuted));
                    return _clearLeechListCommand;
                }
            }
        }

        private void ClearLeechListExecuted()
        {
            LeechList.Clear();
        }

        private List<Proxy> GetIpsFromContent(string content)
        {
            // http://www.regular-expressions.info/examples.html
            Regex regex = new Regex(@"\b(?:\d{1,3}\.){3}\d{1,3}:[0-9]{1,5}\b");
            MatchCollection matches = regex.Matches(content);

            List<Proxy> proxys = new List<Proxy>();
            foreach (Match match in matches)
            {
                Proxy p = new Proxy
                {
                    // Proxys default values
                    Ping = 0,
                    Speed = 0
                };

                string m = match.ToString();
                if (m.Contains(":"))
                {
                    p.IP = m.Substring(0, m.IndexOf(':'));
                    p.Port = m.Substring(m.IndexOf(':') + 1);
                }
                else
                {
                    p.IP = m;
                    // Default HTTP Port for Proxys with no Port found
                    p.Port = "80";
                }

                if (!proxys.Exists(v => v.IP == p.IP))
                {
                    proxys.Add(p);
                }
            }
            return proxys;
        }

        private RelayCommand<IList> _leechListSelectionChangedCommand;
        public RelayCommand<IList> LeechListSelectionChangedCommand
        {
            get
            {
                return _leechListSelectionChangedCommand ?? (_leechListSelectionChangedCommand = new RelayCommand<IList>(LeechListSelectionChanged));
            }
        }

        private void LeechListSelectionChanged(IList items)
        {
            SelectedUrls.Clear();
            foreach (var item in items)
            {
                SelectedUrls.Add((ProxyLeechListModel)item);
            }
        }

        private RelayCommand _clearSelectedLeechListCommand;
        public RelayCommand ClearSelectedLeechListCommand
        {
            get
            {
                {
                    if (_clearSelectedLeechListCommand == null)
                        _clearSelectedLeechListCommand = new RelayCommand(new Action(ClearSelectedLeechListExecuted));
                    return _clearSelectedLeechListCommand;
                }
            }
        }

        private void ClearSelectedLeechListExecuted()
        {
            ObservableCollection<ProxyLeechListModel> temp = new ObservableCollection<ProxyLeechListModel>();
            foreach (var item in LeechList)
            {
                if (!SelectedUrls.Contains(item))
                {
                    temp.Add(item);
                }
            }
            LeechList.Clear();
            foreach (var item in temp)
            {
                LeechList.Add(item);
            }
        }

        private RelayCommand _pasteLeechListCommand;
        public RelayCommand PasteLeechListCommand
        {
            get
            {
                {
                    if (_pasteLeechListCommand == null)
                        _pasteLeechListCommand = new RelayCommand(new Action(PasteLeechListExecuted));
                    return _pasteLeechListCommand;
                }
            }
        }

        private void PasteLeechListExecuted()
        {
            ReadLeechFile(Clipboard.GetText());
        }
    }
}