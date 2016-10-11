using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace QCloud.WeApp.Demo.MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            SDK.ConfigurationManager.Setup(new SDK.Configuration()
            {
                ServerHost = "csharp-demo.qcloud.la",
                AuthServerUrl = "http://10.104.175.21/mina_auth/",
                SecretKey = "flydeGe&g^2",
                TunnelServerUrl = "https://ws.qcloud.com"
            });
        }
    }
}
