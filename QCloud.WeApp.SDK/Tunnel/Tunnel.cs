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

        public static void Broadcast(IEnumerable<string> tunnelIds, string type, object message)
        {

        }

        public static void Broadcast(IEnumerable<Tunnel> tunnels, string type, object message) {
            Broadcast(tunnels.Select(x => x.Id), type, message);
        }

        public string Id { get; internal set; }

        internal string ConnectUrl { get; set; }
        
        public void Emit(string type, object message = null) {
        }
    }
}
