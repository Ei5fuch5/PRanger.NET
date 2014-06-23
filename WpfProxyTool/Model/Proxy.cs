using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfProxyTool.Model
{
    public class Proxy
    {
        public string IP { get; set; }
        public string Port { get; set; }
        public bool Anonym { get; set; }
        public int Ping { get; set; }
        public int Speed { get; set; }
    }
}
