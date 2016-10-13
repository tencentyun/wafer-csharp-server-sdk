using QCloud.WeApp.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.Demo.MVC
{
    public class QCloudConfig
    {
        public static void Setup()
        {
            ConfigurationManager.Setup(new Configuration()
            {
                ServerHost = "csharp-demo.qcloud.la",
                AuthServerUrl = "http://10.104.175.21/mina_auth/",
                SecretKey = "flydeGe&g^2",
                TunnelServerUrl = "http://ws.qcloud.com"
            });
        }
    }
}
