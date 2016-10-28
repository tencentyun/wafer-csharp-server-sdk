using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QCloud.WeApp.SDK.Tunnel
{
    /// <summary>
    /// 表示一个信道消息
    /// </summary>
    public class TunnelMessage
    {
        internal TunnelMessage(string messageRaw)
        {
            try
            {
                dynamic resolved = JsonConvert.DeserializeObject(messageRaw);
                Type = resolved.type;
                Content = resolved.content;
            }
            catch
            {
                Type = "UnknownRaw";
                Content = messageRaw;
            }
        }

        /// <summary>
        /// 消息的类型
        /// </summary>
        public string Type { get; internal set; }

        /// <summary>
        /// 消息的内容
        /// </summary>
        public dynamic Content { get; internal set; }
    }
}
