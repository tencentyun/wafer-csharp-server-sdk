using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK.Tunnel
{
    public class TunnelHandleOptions
    {
        /// <summary>
        /// 建立隧道连接的时候是否要检查登录，设置为 true 则可以获取已登录用户的用户信息
        /// </summary>
        public bool CheckLogin { get; set; }
    }
}
