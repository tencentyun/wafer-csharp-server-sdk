using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK
{
    public static class ConfigurationManager
    {
        private static Configuration _CurrentConfiguration;

        public static Configuration CurrentConfiguration
        {
            get
            {
                if (_CurrentConfiguration == null)
                {
                    throw new Exception("SDK 还没有进行配置，请调用 ConfigurationManager.Setup() 方法配置 SDK");
                }
                return _CurrentConfiguration;
            }
        }
        public static void Setup(Configuration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration), "配置不能为空");
            if (configuration.SecretKey == null) throw new ArgumentNullException(nameof(configuration.SecretKey), "SDK 密钥配置不能为空");
            if (configuration.ServerHost == null) throw new ArgumentNullException(nameof(configuration.ServerHost), "服务器主机配置不能为空");
            if (configuration.AuthServerUrl == null) throw new ArgumentNullException(nameof(configuration.AuthServerUrl), "鉴权服务器配置不能为空");
            if (configuration.TunnelServerUrl == null) throw new ArgumentNullException(nameof(configuration.TunnelServerUrl), "信道服务器配置不能为空");
            _CurrentConfiguration = configuration;
        }
    }
}
