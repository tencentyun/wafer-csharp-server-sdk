using QCloud.WeApp.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.Demo.MVC.Business
{

    class TunnelHandler : ITunnelHandler
    {
        private static Dictionary<string, UserInfo> userMap = new Dictionary<string, UserInfo>();

        public void OnTunnelRequest(Tunnel tunnel, UserInfo user)
        {
            userMap.Add(tunnel.Id, user);
        }

        public void OnTunnelConnect(Tunnel tunnel)
        {
            throw new NotImplementedException();
        }

        public void OnTunnelMessage(Tunnel tunnel, TunnelMessage message)
        {
            throw new NotImplementedException();
        }

        public void OnTunnelClose(Tunnel tunnel)
        {
            throw new NotImplementedException();
        }
    }
}
