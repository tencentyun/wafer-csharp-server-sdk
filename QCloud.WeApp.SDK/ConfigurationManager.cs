using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
            if (configuration == null) throw new ConfigurationException("配置不能为空", new ArgumentNullException(nameof(configuration)));
            if (configuration.ServerHost == null) throw new ConfigurationException("服务器主机配置不能为空", new ArgumentNullException(nameof(configuration.ServerHost)));
            if (configuration.AuthServerUrl == null) throw new ConfigurationException("鉴权服务器配置不能为空", new ArgumentNullException(nameof(configuration.AuthServerUrl)));
            if (configuration.TunnelServerUrl == null) throw new ConfigurationException("信道服务器配置不能为空", new ArgumentNullException(nameof(configuration.TunnelServerUrl)));
            if (configuration.TunnelSignatureKey == null) throw new ConfigurationException("信道服务密钥配置不能为空", new ArgumentNullException(nameof(configuration.TunnelSignatureKey)));
            _CurrentConfiguration = configuration;
        }
        public static void SetupFromFile(string configFilePath)
        {
            if (!File.Exists(configFilePath)) throw new ConfigurationException($"配置文件不存在：{configFilePath}");

            try
            {
                string configFileContent = File.ReadAllText(configFilePath);
                Setup(JsonConvert.DeserializeObject<Configuration>(configFileContent));
            }
            catch (IOException)
            {
                throw new ConfigurationException("配置文件读取失败，请检查是否有足够的权限");
            }
        }
    }
}
