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
    public class TunnelService
    {
        private HttpRequestBase Request;
        private HttpResponseBase Response;

        /// <summary>
        /// 提供隧道服务
        /// </summary>
        public TunnelService(HttpRequestBase request, HttpResponseBase response)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", "初始化登录服务时，request 不能为 null");
            }
            if (response == null)
            {
                throw new ArgumentNullException("response", "初始化登录服务时，response 不能为 null");
            }
            this.Request = request;
            this.Response = response;
        }

        /// <summary>
        /// 提供隧道服务
        /// </summary>
        public TunnelService(HttpRequest request, HttpResponse response) : this(new HttpRequestWrapper(request), new HttpResponseWrapper(response)) { }
        
        public async Task Handle(ITunnelHandler handler, TunnelHandleOptions options = null)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                await HandleGet(handler, options);
            }

            if (Request.HttpMethod.ToUpper() == "POST")
            {
                await HandlePost(handler, options);
            }
        }

        private async Task HandleGet(ITunnelHandler handler, TunnelHandleOptions options)
        {
            TunnelAPI tunnelApi = new TunnelAPI();

            UserInfo user = null;
            Tunnel tunnel = null;

            if (options?.CheckLogin == true)
            {
                try
                {
                    LoginService loginService = new LoginService(Request, Response);
                    user = await loginService.Check();
                }
                catch {
                    return;
                }
            }

            tunnel = await tunnelApi.RequestConnect("kdi309c32", "http://csharp-demo.qcloud.la/tunnel");

            Response.WriteJson(new
            {
                url = tunnel.ConnectUrl
            });

            handler.OnTunnelRequest(tunnel, user);
        }

        private async Task HandlePost(ITunnelHandler handler, TunnelHandleOptions options)
        {
            Request.SaveAs($"C:\\requests\\{DateTime.Today.ToString("yyyyMMdd_HH_mm_ss")}", true);
            throw new NotImplementedException();
        }
    }
}
