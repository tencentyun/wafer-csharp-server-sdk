using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK
{
    public class Tunnel
    {
        public string Id { get; internal set; }

        public string ConnectUrl { get; internal set; }
        
        public void Emit(string type, object message) {
        }
    }
}
