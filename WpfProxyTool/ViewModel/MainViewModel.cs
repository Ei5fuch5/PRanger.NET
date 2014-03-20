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
        public ObservableCollection<ProxyLeecherModel> leechList { get; set; }
        private static object listLock = new object();

        private string leechTimeout;
        public string LeechTimeout
        {
            get
            {
                return leechTimeout;
            }
            set
            {
                leechTimeout = value;
                OnPropertyChanged("LeechTimeout");
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            leechList = new ObservableCollection<ProxyLeecherModel>();
            BindingOperations.EnableCollectionSynchronization(leechList, listLock);
            leechTimeout = "5";
            /*
            leechList.Add(new ProxyLeecherModel
                {
                    URL = "http://google.de"
                });
            */
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        
        private RelayCommand openLeechListCommand;
        public RelayCommand OpenLeechListCommand
        {
            get
            {
                {
                    if (openLeechListCommand == null)
                        openLeechListCommand = new RelayCommand(new Action(openLeechListExecuted));
                    return openLeechListCommand;
                }
            }
        }

        private void openLeechListExecuted()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.InitialDirectory = @"C:\";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Task.Factory.StartNew( () => readLeechFile(ofd.FileName));
                //MessageBox.Show(ofd.FileName);
            }
        }

        private void readLeechFile(String path)
        {
            string file = System.IO.File.ReadAllText(path);
            Regex regex = new Regex(@"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?");
            MatchCollection matches = regex.Matches(file);
            foreach (Match match in matches)
            {
                leechList.Add(new ProxyLeecherModel
                    {
                        URL = match.ToString()
                    }
                );
            }
        }

        private RelayCommand<DragEventArgs> dataGridLeecherDropCommand;
        public RelayCommand<DragEventArgs> DataGridLeecherDropCommand
        {
            get
            {
                return dataGridLeecherDropCommand ?? (dataGridLeecherDropCommand = new RelayCommand<DragEventArgs>(dataGridLeecherDrop));
            }
        }
        private void dataGridLeecherDrop(DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            Task.Factory.StartNew(() => readLeechFile(files[0]));
            // do something here
        }


        private RelayCommand startLeechingCommand;
        public RelayCommand StartLeechingCommand
        {
            get
            {
                {
                    if (startLeechingCommand == null)
                        startLeechingCommand = new RelayCommand(new Action(startLeechingExecuted));
                    return startLeechingCommand;
                }
            }
        }

        private async void startLeechingExecuted()
        {
            MessageBox.Show(leechTimeout);
            foreach (ProxyLeecherModel item in leechList)
            {
                WebContent source = new WebContent();
                source = await getURLContentAsync(item.URL);

                string content = System.Text.Encoding.Default.GetString(source.content);
                item.Proxys.Add(content);
                item.Date = DateTime.Now;
                item.Count = source.content.Length;
                item.Reply = source.status;
            }
            
        }

        private async Task<WebContent> getURLContentAsync(string url)
        {
            WebContent result = new WebContent();
            try
            {
                var content = new MemoryStream();
                var webReq = (HttpWebRequest)WebRequest.Create(url);
                using (WebResponse response = await webReq.GetResponseAsync())
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
    }
}