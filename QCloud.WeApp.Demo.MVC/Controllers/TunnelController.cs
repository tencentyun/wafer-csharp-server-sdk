using QCloud.WeApp.Demo.MVC.Business;
using QCloud.WeApp.SDK;
using QCloud.WeApp.SDK.Tunnel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QCloud.WeApp.Demo.MVC.Controllers
{

    public class TunnelController : Controller
    {

        private TunnelService tunnelService = null;

        /// <summary>
        /// GET /tunnel <para />
        /// 请求建立隧道连接 <para />
        /// 
        /// POST /tunnel <para />
        /// 用于信道服务器推送消息到业务服务器
        /// </summary>
        public ActionResult Index()
        {

            // 创建信道服务处理信道相关请求
            tunnelService = new TunnelService(Request, Response);

            // 信道服务会自动帮你响应请求，请不要使用 Response 进行二次请求
            tunnelService.Handle(

                // 需要自己实现隧道消息处理器，TunnelHandler 是一个实现的范例，详情请看 TunnelHandler 的实现源码
                handler: new ChatTunnelHandler(), 

                // 配置是可选的，配置 CheckLogin 为 true 的话，会在隧道建立之前获取用户信息，以便业务将隧道和用户关联起来
                options: new TunnelHandleOptions() { CheckLogin = true }
            );

            // 返回 null 确保 MVC 框架不进行输出
            return null;
        }
    }
}