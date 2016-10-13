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
                //AuthServerUrl = "http://10.104.175.21/mina_auth/",
                AuthServerUrl = "http://mina.auth.com:9447/",
                SecretKey = "flydeGe&g^2",
                TunnelServerUrl = "http://ws.qcloud.com",
                NetworkProxy = "http://127.0.0.1:8888"
            });
        }
    }
}
