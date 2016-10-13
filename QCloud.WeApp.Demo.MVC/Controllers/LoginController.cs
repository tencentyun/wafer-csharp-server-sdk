using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QCloud.WeApp.SDK;
using System.Threading.Tasks;
using System.Diagnostics;

namespace QCloud.WeApp.Demo.MVC.Controllers
{
    public class LoginController : Controller
    {
        /// <summary>
        /// GET /login <para />
        /// 实现登录接口，配合客户端 SDK 建立会话
        /// </summary>
        public ActionResult Index()
        {
            try
            {
                // 使用 Request 和 Response 初始化登录服务
                LoginService loginService = new LoginService(Request, Response);

                // 调用登录接口，如果成功可以获得用户信息。如有需要，可以使用用户信息进行进一步的业务操作
                UserInfo userInfo = loginService.Login();

                Debug.WriteLine(userInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            // 登录无论成功还是失败，都无需返回响应结果，因为登录服务已经使用 HTTP Response 进行输出
            return null;
        }
    }
}