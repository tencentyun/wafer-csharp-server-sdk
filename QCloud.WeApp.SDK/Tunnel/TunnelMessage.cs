using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QCloud.WeApp.SDK.Tunnel
{
    public class TunnelMessage
    {
        internal TunnelMessage(string messageRaw)
        {
            try
            {
                dynamic resolved = JsonConvert.DeserializeObject(messageRaw);
                Type = resolved.type;
                Content = resolved.content;
            }
            catch
            {
                Type = "UnknownRaw";
                Content = messageRaw;
            }
        }

        public string Type { get; internal set; }

        public dynamic Content { get; internal set; }
    }
}
