using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfProxyTool.Model
{
    public class ProxyLeechListModel : System.ComponentModel.INotifyPropertyChanged
    {
        public ProxyLeechListModel()
        {
            proxys = new List<string>();
        }

        // Need to notify the WPF elements if any of the properties changed on a Person object
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        private string url;
        public string URL
        {
            get
            {
                return url;
            }
            set
            {
                url = value;
                OnPropertyChanged("URL");
            }
        }


        private string reply;
        public string Reply
        {
            get
            {
                return reply;
            }
            set
            {
                reply = value;
                OnPropertyChanged("Reply");
            }
        }


        private int count;
        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
                OnPropertyChanged("Count");
            }
        }


        private DateTime date;
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }

        private List<String> proxys;
        public List<String> Proxys
        {
            get
            {
                return proxys;
            }
            set
            {
                proxys = value;
                OnPropertyChanged("Proxys");
            }
        }
    }
}
