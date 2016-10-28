using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK.Tunnel
{
    /// <summary>
    /// 表示一条无效信道的信息
    /// </summary>
    /// <remarks>每次向信道服务器发送消息，信道服务器都会告知无效的信道列表。业务服务器可以根据这个列表清理无效的信道。</remarks>
    public class TunnelInvalidInfo
    {
        /// <summary>
        /// 无效信道的 ID
        /// </summary>
        public string TunnelId { get; internal set; }

        /// <summary>
        /// 无效的类型
        /// </summary>
        public TunnelInvalidType Type { get; set; }
    }
}
