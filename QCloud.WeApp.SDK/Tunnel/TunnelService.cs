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
    /// <summary>
    /// 提供 WebSocket 信道服务
    /// </summary>
    public partial class TunnelService
    {
        private HttpRequestBase Request;
        private HttpResponseBase Response;

        /// <summary>
        /// 提供 WebSocket 信道服务
        /// </summary>
        public TunnelService(HttpRequestBase request, HttpResponseBase response)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", "初始化 WebSocket 信道服务时，request 不能为 null");
            }
            if (response == null)
            {
                throw new ArgumentNullException("response", "初始化 WebSocket 信道服务时，response 不能为 null");
            }
            Request = request;
            Response = response;
        }

        /// <summary>
        /// 提供 WebSocket 信道服务
        /// </summary>
        public TunnelService(HttpRequest request, HttpResponse response) : this(new HttpRequestWrapper(request), new HttpResponseWrapper(response)) { }
        
        /// <summary>
        /// 处理 WebSocket 信道请求
        /// </summary>
        /// <param name="handler">提供 WebSocket 信道处理器处理信道事件</param>
        /// <param name="options">可选，配置处理选项</param>
        /// <returns>返回任务进行跟踪，任务完成表示信道处理完成</returns>
        public void Handle(ITunnelHandler handler, TunnelHandleOptions options = null)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                HandleGet(handler, options);
            }

            if (Request.HttpMethod.ToUpper() == "POST")
            {
                HandlePost(handler, options);
            }
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
