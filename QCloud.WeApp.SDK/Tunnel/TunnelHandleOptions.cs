using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK.Tunnel
{
    /// <summary>
    /// 表示处理信道请求时，指定的配置
    /// </summary>
    public class TunnelHandleOptions
    {
        /// <summary>
        /// 建立信道连接前是否要求登录，设置为 true 则可以获取已登录用户的用户信息
        /// </summary>
        public bool CheckLogin { get; set; }
    }
}
