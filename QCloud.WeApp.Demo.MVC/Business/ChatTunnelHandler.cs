using QCloud.WeApp.SDK;
using QCloud.WeApp.SDK.Authorization;
using QCloud.WeApp.SDK.Tunnel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.Demo.MVC.Business
{
    /// <summary>
    /// 实现 WebSocket 信道处理器
    /// 本示例配合客户端 Demo 实现一个简单的聊天室功能
    /// 
    /// 信道处理器需要处理信道的完整声明周期，包括：
    ///     onTunnelRequest() - 当用户发起信道请求的时候，会得到用户信息，此时可以关联信道 ID 和用户信息
    ///     onTunnelConnect() - 当用户建立了信道连接之后，可以记录下已经连接的信道
    ///     onTunnelMessage() - 当用户消息发送到信道上时，使用该函数处理信道的消息
    ///     onTunnelClose() -   当信道关闭时，清理关于该信道的信息，以及回收相关资源
    /// </summary>
    class ChatTunnelHandler : ITunnelHandler
    {
        /// <summary>
        /// 记录 WebSocket 信道对应的用户。在实际的业务中，应该使用数据库进行存储跟踪，这里作为示例只是演示其作用
        /// </summary>
        private static Dictionary<string, UserInfo> userMap = new Dictionary<string, UserInfo>();

        /// <summary>
        /// 创建一个房间，包含当前已连接的 WebSocket 信道列表
        /// </summary>
        private static TunnelRoom room = new TunnelRoom();


        /// <summary>
        /// 实现 OnTunnelRequest 方法
        /// 在客户端请求 WebSocket 信道连接之后，会调用 OnTunnelRequest 方法，此时可以把信道 ID 和用户信息关联起来
        /// </summary>
        /// <param name="tunnel">客户端请求道的 WebSocket 信道</param>
        /// <param name="user">客户端当前会话用户</param>
        public void OnTunnelRequest(Tunnel tunnel, UserInfo user)
        {
            if (user != null)
            {
                userMap.Add(tunnel.Id, user);
            }
        }

        /// <summary>
        /// 实现 OnTunnelConnect 方法
        /// 在客户端成功连接 WebSocket 信道服务之后会调用该方法，此时通知所有其它在线的用户当前总人数以及刚加入的用户是谁
        /// </summary>
        /// <param name="tunnel">成功连接的 WebSocket 信道</param>
        public void OnTunnelConnect(Tunnel tunnel)
        {
            if (userMap.ContainsKey(tunnel.Id))
            {
                room.AddTunnel(tunnel);
                room.Broadcast("people", new { total = room.TunnelCount, enter = userMap[tunnel.Id] });
            }
            else
            {
                tunnel.Close();
            }
        }

        /// <summary>
        /// 实现 OnTunnelMessage 方法
        /// 客户端推送消息到 WebSocket 信道服务器上后，会调用该方法，此时可以处理信道的消息。
        /// 在本示例，我们处理 "speak" 类型的消息，该消息表示有用户发言。我们把这个发言的信息广播到所有在线的 WebSocket 信道上
        /// </summary>
        /// <param name="tunnel">发送消息的信道</param>
        /// <param name="message">发送的消息数据</param>
        public void OnTunnelMessage(Tunnel tunnel, TunnelMessage message)
        {
            switch(message.Type)
            {
                // 目前只处理 "speak" 类型的消息，只要有客户端说话，就把这个消息广播到所有信道上
                case "speak":
                    if (userMap.ContainsKey(tunnel.Id))
                    {
                        Broadcast("speak", new { who = userMap[tunnel.Id], word = message.Content.word });                        
                    }
                    else
                    {
                        tunnel.Close();
                    }
                    break;
            }
        }

        /// <summary>
        /// 实现 OnTunnelClose 方法
        /// 客户端关闭 WebSocket 信道或者被信道服务器判断为已断开后，会调用该方法，此时可以进行清理及通知操作
        /// </summary>
        /// <param name="tunnel">已关闭的信道</param>
        public void OnTunnelClose(Tunnel tunnel)
        {
            UserInfo leaveUser = null;
            if (userMap.ContainsKey(tunnel.Id))
            {
                leaveUser = userMap[tunnel.Id];
                userMap.Remove(tunnel.Id);
            }
            room.RemoveTunnel(tunnel);
            Broadcast("people", new { total = room.TunnelCount, leave = leaveUser });
        }

        /// <summary>
        /// 广播消息到房间里所有的信道
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="messageContent"></param>
        void Broadcast(string messageType, object messageContent)
        {
            try
            {
                EmitResult broadcastResult = room.Broadcast(messageType, messageContent);
                // 可以检测无效信道并进行清理
                if (broadcastResult.HasTunnelInvalidInfo)
                {
                    foreach (var invalidInfo in broadcastResult.TunnelIvalidInfos)
                    {
                        OnTunnelClose(Tunnel.GetById(invalidInfo.TunnelId));
                    }
                }
            }
            catch (Exception error)
            {
                // 如果消息发送发生异常，这里可以进行错误处理或者重试的逻辑
                Debug.WriteLine(error);
            }
        }
    }
}
