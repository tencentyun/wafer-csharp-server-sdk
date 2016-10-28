using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK.Tunnel
{
    /// <summary>
    /// 表示一个 WebSocket 信道
    /// </summary>
    /// <remarks>
    /// 信道不能创建，可以通过静态方法 Tunnel.GetById 获取具有指定 ID 的信道
    /// </remarks>
    public class Tunnel
    {
        /// <summary>
        /// 根据信道 ID 获取信道
        /// </summary>
        /// <param name="tunnelId"></param>
        /// <returns></returns>
        public static Tunnel GetById(string tunnelId) {
            return new Tunnel(tunnelId);
        }

        /// <summary>
        /// 根据信道 ID 获取实例化信道
        /// </summary>
        /// <param name="tunnelId"></param>
        internal Tunnel(string tunnelId)
        {
            if (tunnelId == null) throw new ArgumentNullException("tunnelId", "信道 ID 不能为空！");
            Id = tunnelId;
        }

        /// <summary>
        /// 信道的唯一 ID
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// 信道的连接地址
        /// </summary>
        internal string ConnectUrl { get; set; }
        
        /// <summary>
        /// 通过信道发送消息
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="messageContent">消息内容，可以为任意类型，不过对象必须可以被序列化，建议使用 JSONObject</param>
        /// <returns>返回信道发送结果，可能包含无效信道列表</returns>
        /// <exception cref="EmitException">网络不可用或信道服务器不可用</exception>
        public EmitResult Emit(string messageType, object messageContent = null) {
            TunnelAPI api = new TunnelAPI();
            return api.EmitMessage(new string[] { Id }, messageType, messageContent);
        }

        /// <summary>
        /// 关闭当前信道
        /// </summary>
        /// <returns>返回信道关闭送结果，可能包含无效信道列表</returns>
        /// <exception cref="EmitException">网络不可用或信道服务器不可用</exception>
        public EmitResult Close()
        {
            TunnelAPI api = new TunnelAPI();
            return api.EmitPacket(new string[] { Id }, "close", null);
        }
    }
}
