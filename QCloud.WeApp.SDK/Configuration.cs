using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK
{
    /// <summary>
    /// 表示 SDK 配置。想要进行配置，需要使用 <c>ConfigurationManager</c>
    /// </summary>
    /// <see cref="ConfigurationManager" />
    [JsonObject]
    public class Configuration
    {

        /// <summary>
        /// 当前使用 SDK 服务器的主机，该主机需要外网可访问。信道服务器依赖该主机名推送信道消息到业务服务器。
        /// </summary>
        [JsonProperty("serverHost")]
        public string ServerHost { get; set; }

        /// <summary>
        /// 鉴权服务器服务地址
        /// </summary>
        [JsonProperty("authServerUrl")]
        public string AuthServerUrl { get; set; }

        /// <summary>
        /// 信道服务器服务地址
        /// </summary>
        [JsonProperty("tunnelServerUrl")]
        public string TunnelServerUrl { get; set; }

        /// <summary>
        /// 与信道服务器通信时签名使用的密钥
        /// </summary>
        [JsonProperty("tunnelSignatureKey")]
        public string TunnelSignatureKey { get; set; }

        /// <summary>
        /// 网络请求代理地址
        /// </summary>
        [JsonProperty("networkProxy")]
        public string NetworkProxy { get; set; }

        /// <summary>
        /// 网络超时设置，单位为秒
        /// </summary>
        [DefaultValue(15)]
        [JsonProperty("networkTimeout", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int NetworkTimeout { get; internal set; }
    }
}
