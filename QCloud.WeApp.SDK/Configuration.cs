using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK
{
    /// <summary>
    /// 表示 SDK 配置
    /// </summary>
    public class Configuration
    {

        /// <summary>
        /// SDK 密钥，该密钥需要保密
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 当前使用 SDK 服务器的主机，该主机需要外网可访问
        /// </summary>
        public string ServerHost { get; set; }

        /// <summary>
        /// 鉴权服务器服务地址
        /// </summary>
        public string AuthServerUrl { get; set; }

        /// <summary>
        /// 信道服务器服务地址
        /// </summary>
        public string TunnelServerUrl { get; set; }
        
        /// <summary>
        /// 网络请求代理地址
        /// </summary>
        public string NetworkProxy { get; set; }
    }
}
