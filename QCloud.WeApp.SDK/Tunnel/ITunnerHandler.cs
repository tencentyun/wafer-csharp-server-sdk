using QCloud.WeApp.SDK.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK.Tunnel
{
    public interface ITunnelHandler
    {
        void OnTunnelRequest(Tunnel tunnel, UserInfo user);
        void OnTunnelConnect(Tunnel tunnel);
        void OnTunnelMessage(Tunnel tunnel, TunnelMessage message);
        void OnTunnelClose(Tunnel tunnel);
    }
}
