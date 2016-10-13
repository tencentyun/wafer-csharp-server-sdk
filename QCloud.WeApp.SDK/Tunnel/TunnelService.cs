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
    }
}
