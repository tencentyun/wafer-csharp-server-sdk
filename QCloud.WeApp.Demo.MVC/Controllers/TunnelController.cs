using QCloud.WeApp.SDK;
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
        /// 推送消息到隧道
        /// </summary>
        public async Task<ActionResult> Index()
        {
            tunnelService = new TunnelService(Request, Response);
            await tunnelService.Handle(new Business.TunnelHandler(), new TunnelHandleOptions() { CheckLogin = true });
            return null;
        }
    }
}