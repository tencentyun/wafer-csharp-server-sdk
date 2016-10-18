using Newtonsoft.Json;
using QCloud.WeApp.SDK.Authorization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QCloud.WeApp.SDK.Tunnel
{

    public partial class TunnelService
    {

        /// <summary>
        /// 处理 WebSocket 信道 GET 请求
        /// </summary>
        /// 
        /// <remarks>
        /// GET 请求表示客户端请求进行信道连接，此时会向 SDK 申请信道连接地址，并且返回给客户端
        /// 如果配置指定了要求登陆，还会调用登陆服务来校验登陆态并获得用户信息
        /// </remarks>
        private void HandleGet(ITunnelHandler handler, TunnelHandleOptions options)
        {

            Configuration config = ConfigurationManager.CurrentConfiguration;

            Tunnel tunnel = null;
            UserInfo user = null;

            // 要求登录态，获取用户信息
            if (options?.CheckLogin == true)
            {
                try
                {
                    LoginService loginService = new LoginService(Request, Response);
                    user = loginService.Check();
                }
                catch
                {
                    // 要求检查登录态的话，发生异常就结束了
                    return;
                }
            }

            // 申请 WebSocket 信道连接地址
            TunnelAPI tunnelApi = new TunnelAPI();
            try
            {
                tunnel = tunnelApi.RequestConnect(BuildReceiveUrl());
            }
            catch (Exception e)
            {
                Response.WriteJson(new { error = e.Message });
                throw e;
            }

            // 输出 URL 结果
            Response.WriteJson(new { url = tunnel.ConnectUrl });

            handler.OnTunnelRequest(tunnel, user);
        }
        
    }
}
