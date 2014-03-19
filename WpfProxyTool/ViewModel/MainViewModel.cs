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

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            leechList = new ObservableCollection<ProxyLeecherModel>();
            BindingOperations.EnableCollectionSynchronization(leechList, listLock);
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
            Regex regex = new Regex(@"\b(((\S+)?)(@|mailto\:|(news|(ht|f)tp(s?))\://)\S+)\b");
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
    }
}