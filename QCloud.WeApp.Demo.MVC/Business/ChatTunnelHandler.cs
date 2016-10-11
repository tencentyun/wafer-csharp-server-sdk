using QCloud.WeApp.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.Demo.MVC.Business
{
    /// <summary>
    /// 实现 WebSocket 信道处理器
    /// 本示例配合客户端 Demo 实现一个简单的聊天室功能
    /// </summary>
    class ChatTunnelHandler : ITunnelHandler
    {
        /// <summary>
        /// 记录 WebSocket 信道对应的用户。在实际的业务中，应该使用数据库进行存储跟踪，这里作为示例只是演示其作用
        /// </summary>
        private static Dictionary<string, UserInfo> userMap = new Dictionary<string, UserInfo>();

        /// <summary>
        /// 当前已连接的 WebSocket 信道列表
        /// </summary>
        private static List<Tunnel> connectedTunnels = new List<Tunnel>();


        /// <summary>
        /// 实现 OnTunnelRequest 方法
        /// 在客户端请求 WebSocket 信道连接之后，会调用 OnTunnelRequest 方法，此时可以把信道 ID 和用户信息关联起来
        /// </summary>
        /// <param name="tunnel">客户端请求道的 WebSocket 信道</param>
        /// <param name="user">客户端当前会话用户</param>
        void ITunnelHandler.OnTunnelRequest(Tunnel tunnel, UserInfo user)
        {
            userMap.Add(tunnel.Id, user);
        }

        /// <summary>
        /// 实现 OnTunnelConnect 方法
        /// 在客户端成功连接 WebSocket 信道服务之后会调用该方法，此时通知所有其它在线的用户当前总人数以及刚加入的用户是谁
        /// </summary>
        /// <param name="tunnel">成功连接的 WebSocket 信道</param>
        void ITunnelHandler.OnTunnelConnect(Tunnel tunnel)
        {
            connectedTunnels.Add(tunnel);
            Broadcast("people", new { total = connectedTunnels.Count, enter = userMap[tunnel.Id] });
            tunnel.Emit("hello");
        }

        /// <summary>
        /// 实现 OnTunnelMessage 方法
        /// 客户端推送消息到 WebSocket 信道服务器上后，会调用该方法，此时可以处理信道的消息。
        /// 在本示例，我们处理 "speak" 类型的消息，该消息表示有用户发言。我们把这个发言的信息广播到所有在线的 WebSocket 信道上
        /// </summary>
        /// <param name="tunnel">发送消息的信道</param>
        /// <param name="message">发送的消息数据</param>
        void ITunnelHandler.OnTunnelMessage(Tunnel tunnel, TunnelMessage message)
        {
            switch(message.Type)
            {
                // 目前只处理 "speak" 类型的消息，只要有客户端说话，就把这个消息广播到所有信道上
                case "speak":
                    Broadcast("speak", new { who = userMap[tunnel.Id], word = message.Message.word });
                    break;
            }
        }

        /// <summary>
        /// 实现 OnTunnelClose 方法
        /// 客户端关闭 WebSocket 信道或者被信道服务器判断为已断开后，会调用该方法，此时可以进行清理及通知操作
        /// </summary>
        /// <param name="tunnel">已关闭的信道</param>
        void ITunnelHandler.OnTunnelClose(Tunnel tunnel)
        {
            UserInfo leaveUser = null;
            if (userMap.ContainsKey(tunnel.Id))
            {
                leaveUser = userMap[tunnel.Id];
                userMap.Remove(tunnel.Id);
            }
            connectedTunnels.RemoveAll(x => x.Id == tunnel.Id);
            Broadcast("people", new { total = connectedTunnels.Count, leave = leaveUser });
        }

        /// <summary>
        /// 调用 Tunnel.Broadcast() 进行广播
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        private void Broadcast(string type, object message)
        {
            Tunnel.Broadcast(connectedTunnels, type, message);
        }
    }
}
