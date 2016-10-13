using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK
{
    public class TunnelRoom
    {
        private List<Tunnel> Tunnels;

        public TunnelRoom() : this(null) { }

        public TunnelRoom(IEnumerable<Tunnel> tunnels)
        {
            Tunnels = tunnels == null ? new List<Tunnel>() : tunnels.ToList();
        }

        public void AddTunnel(Tunnel tunnel)
        {
            Tunnels.Add(tunnel);
        }

        public void RemoveTunnel(Tunnel tunnel)
        {
            this.RemoveTunnelById(tunnel.Id);
        }

        public void RemoveTunnelById(string tunnelId)
        {
            Tunnels.RemoveAll(x => x.Id == tunnelId);
        }

        public int TunnelCount
        {
            get
            {
                return Tunnels.Count;
            }
        }

        public bool Broadcast(string messageType, object messageContent)
        {
            if (Tunnels != null)
            {
                TunnelAPI api = new TunnelAPI();
                return api.EmitMessage(Tunnels.Select(x => x.Id), messageType, messageContent);
            }
            return true;
        }
    }
}
