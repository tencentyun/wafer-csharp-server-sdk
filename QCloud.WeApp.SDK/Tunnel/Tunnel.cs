using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK
{
    public class Tunnel
    {
        public static Tunnel GetById(string tunnelId)
        {
            return new Tunnel() { Id = tunnelId };
        }
        
        public string Id { get; internal set; }

        internal string ConnectUrl { get; set; }
        
        public bool Emit(string messageType, object messageContent = null) {
            TunnelAPI api = new TunnelAPI();
            return api.EmitMessage(new string[] { Id }, messageType, messageContent);
        }

        public bool Close()
        {
            TunnelAPI api = new TunnelAPI();
            return api.EmitPacket(new string[] { Id }, "close", null);
        }
    }
}
