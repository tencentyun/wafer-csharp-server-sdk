using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QCloud.WeApp.SDK
{
    public class TunnelMessage
    {
        public string Type { get; internal set; }
        public dynamic Message { get; internal set; }
    }
}
