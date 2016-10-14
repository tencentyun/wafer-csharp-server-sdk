using QCloud.WeApp.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace QCloud.WeApp.Demo.MVC
{
    public class QCloudConfig
    {
        public static void Setup()
        {
            SDK.ConfigurationManager.SetupFromFile(System.Configuration.ConfigurationManager.AppSettings["qcloud-config-path"]);
        }
    }
}
