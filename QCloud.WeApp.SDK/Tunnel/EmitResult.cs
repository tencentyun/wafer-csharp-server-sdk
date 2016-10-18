using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK.Tunnel
{
    public class EmitResult
    {
        /// <summary>
        /// 消息发送之后，信道服务器是否有发现已经不可用的信道。开发者可能需要清理这些信道。
        /// </summary>
        public bool HasTunnelInvalidInfo
        {
            get
            {
                return TunnelIvalidInfos.Count() > 0;
            }
        }

        /// <summary>
        /// 状态异常的信道，开发者可能需要清理这些信道
        /// </summary>
        public IEnumerable<TunnelInvalidInfo> TunnelIvalidInfos { get; internal set; }
    }
}
