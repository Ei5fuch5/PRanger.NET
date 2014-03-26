using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfProxyTool.Model
{
    public class LeecherModel : System.ComponentModel.INotifyPropertyChanged
    {
        private bool leechStartButtonEnabled;
        public bool LeechStartButtonEnabled
        {
            get
            {
                return leechStartButtonEnabled;
            }
            set
            {
                leechStartButtonEnabled = value;
                OnPropertyChanged("LeechStartButtonEnabled");
            }
        }

        private bool progressBarEnabled;
        public bool ProgressBarEnabled
        {
            get
            {
                return progressBarEnabled;
            }
            set
            {
                progressBarEnabled = value;
                OnPropertyChanged("ProgressBarEnabled");
            }
        }

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

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
