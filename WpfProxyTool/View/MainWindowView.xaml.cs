using System;
using System.Collections.Generic;
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
using System.Windows.Controls.Ribbon;
using System.IO;

namespace WpfProxyTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : RibbonWindow
    {
        public MainWindowView()
        {
            InitializeComponent();
        }

        private void dataGridLeecher_Drop(object sender, DragEventArgs e)
        {
            String file = e.Data.GetData(DataFormats.Text) as string;
        }
    }
}
