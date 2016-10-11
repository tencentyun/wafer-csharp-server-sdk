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
    /// <summary>
    /// GET /user <para/>
    /// 利用建立的会话获取用户信息
    /// </summary>
    public class UserController : Controller
    {
        // GET: User
        public async Task<ActionResult> Index()
        {
            try
            {
                // 使用 Request 和 Response 初始化登录服务
                LoginService loginService = new LoginService(Request, Response);

                // 调用检查登录接口，成功后可以获得用户信息，进行正常的业务请求
                UserInfo userInfo = await loginService.Check();

                // 获取会话成功，需要返回 HTTP 视图，这里作为示例返回了获得的用户信息
                return Json(new { userInfo }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception error)
            {
                // 可以处理登录失败的情况，但是注意此时无需返回 ActionResult，
                // 因为登录失败的时候，登录服务已经输出登录失败的响应
                Debug.WriteLine(error);
                return null;
            }
        }        
    }
}