using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QCloud.WeApp.SDK
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

            // 申请 WebSocket 信道连接地址
            TunnelAPI tunnelApi = new TunnelAPI();
            try
            {
                var receiveUrl = BuildReceiveUrl();
                tunnel = tunnelApi.RequestConnect(config.SecretKey, receiveUrl);
            }
            catch (Exception e)
            {
                Response.WriteJson(new { error = e.Message });
                throw e;
            }

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

            // 输出 URL 结果
            Response.WriteJson(new { url = tunnel.ConnectUrl });

            handler.OnTunnelRequest(tunnel, user);
        }

        /// <summary>
        /// 构建提交给 WebSocket 信道服务器推送消息的地址
        /// </summary>
        /// 
        /// <remarks>
        /// 构建过程如下：
        ///    1. 从信道服务器地址得到其通信协议（http/https），如 https
        ///    2. 获取当前服务器主机名，如 109447.qcloud.la
        ///    3. 获得当前 HTTP 请求的路径，如 /tunnel
        ///    4. 拼接推送地址为 https://109447.qcloud.la/tunnel
        /// </remarks>
        /// 
        /// <returns>返回构建好的推送消息地址</returns>
        private string BuildReceiveUrl()
        {
            Configuration config = ConfigurationManager.CurrentConfiguration;
            Uri tunnelServerUri = new Uri(config.TunnelServerUrl);
            string schema = tunnelServerUri.Scheme;
            string host = config.ServerHost;
            string path = Request.Url.AbsolutePath;
            UriBuilder builder = new UriBuilder()
            {
                Scheme = schema,
                Host = host,
                Path = path
            };
            return builder.ToString();
        }
        
    }
}
