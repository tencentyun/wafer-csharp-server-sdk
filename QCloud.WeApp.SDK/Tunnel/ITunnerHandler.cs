using QCloud.WeApp.SDK.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK.Tunnel
{
    /// <summary>
    /// 信道事件处理接口，实现该接口处理信道事件
    /// </summary>
    /// <remarks>
    /// 信道处理器需要处理信道的完整声明周期，包括：<para/>
    ///     <c>onTunnelRequest()</c> - 当用户发起信道请求的时候，会得到用户信息，此时可以关联信道 ID 和用户信息<para/>
    ///     <c>onTunnelConnect()</c> - 当用户建立了信道连接之后，可以记录下已经连接的信道<para/>
    ///     <c>onTunnelMessage()</c> - 当用户消息发送到信道上时，使用该函数处理信道的消息<para/>
    ///     <c>onTunnelClose()</c>   - 当信道关闭时，清理关于该信道的信息，以及回收相关资源
    /// </remarks>
    public interface ITunnelHandler
    {
        /// <summary>
        /// 当用户发起信道请求的时候调用，会得到用户信息，此时可以关联信道 ID 和用户信息
        /// </summary>
        /// <param name="tunnel">请求连接的信道</param>
        /// <param name="user">请求信道对应的用户</param>
        /// <remarks>
        /// 如果在使用 LoginService.Handle 没有指定 Login 为 true，则不能获取到用户信息
        /// </remarks>
        void OnTunnelRequest(Tunnel tunnel, UserInfo user);

        /// <summary>
        /// 当用户建立了信道连接之后调用，此时可以记录下已经连接的信道
        /// </summary>
        /// <param name="tunnel">已建立连接的信道</param>
        void OnTunnelConnect(Tunnel tunnel);

        /// <summary>
        /// 当信道收到消息时调用，此时可以处理消息
        /// </summary>
        /// <param name="tunnel">收到消息的信道</param>
        /// <param name="message">收到的消息</param>
        void OnTunnelMessage(Tunnel tunnel, TunnelMessage message);

        /// <summary>
        /// 当信道关闭的时候调用，此时可以清理信道使用的资源
        /// </summary>
        /// <param name="tunnel">关闭的信道</param>
        void OnTunnelClose(Tunnel tunnel);
    }
}
