using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using WpfProxyTool.Model;

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
        public List<ProxyLeecherModel> leechList { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            leechList = new List<ProxyLeecherModel>();
            leechList.Add(new ProxyLeecherModel
                {
                    URL = "http://google.de",
                    Reply = "200",
                    Count = 10,
                    Date = DateTime.Now
                });

            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }
    }
}