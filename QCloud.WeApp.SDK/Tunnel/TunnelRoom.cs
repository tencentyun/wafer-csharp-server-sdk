using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK.Tunnel
{
    /// <summary>
    /// 房间维护一批信道的集合，可以通过广播方法向房间里的所有信道推送消息
    /// </summary>
    public class TunnelRoom
    {
        private List<Tunnel> Tunnels;

        /// <summary>
        /// 创建一个房间，初始包含 0 个信道
        /// </summary>
        public TunnelRoom() : this(null) { }

        /// <summary>
        /// 创建一个房间，初始包含指定的信道列表
        /// </summary>
        /// <param name="tunnels"></param>
        public TunnelRoom(IEnumerable<Tunnel> tunnels)
        {
            Tunnels = tunnels == null ? new List<Tunnel>() : tunnels.ToList();
        }

        /// <summary>
        /// 添加信道到房间
        /// </summary>
        /// <param name="tunnel">要添加的信道</param>
        public void AddTunnel(Tunnel tunnel)
        {
            Tunnels.Add(tunnel);
        }

        /// <summary>
        /// 移除指定信道
        /// </summary>
        /// <param name="tunnel">要移除的信道</param>
        public void RemoveTunnel(Tunnel tunnel)
        {
            this.RemoveTunnelById(tunnel.Id);
        }

        /// <summary>
        /// 根据 ID 移除信道
        /// </summary>
        /// <param name="tunnelId">要移除的信道的 ID</param>
        public void RemoveTunnelById(string tunnelId)
        {
            Tunnels.RemoveAll(x => x.Id == tunnelId);
        }

        /// <summary>
        /// 当前房间包含的信道数量
        /// </summary>
        public int TunnelCount
        {
            get
            {
                return Tunnels.Count;
            }
        }

        /// <summary>
        /// 发送消息到房间里的所有信道
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="messageContent">消息内容，可以为任意类型，不过对象必须可以被序列化</param>
        /// <returns>返回信道发送结果，可能包含无效信道列表</returns>
        /// <exception cref="EmitException">网络不可用或信道服务器不可用</exception>
        public EmitResult Broadcast(string messageType, object messageContent)
        {
            TunnelAPI api = new TunnelAPI();
            return api.EmitMessage(Tunnels.Select(x => x.Id), messageType, messageContent);
        }
    }
}
