using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK
{
    /// <summary>
    /// SDK 配置管理，使用该类对 SDK 进行配置
    /// <seealso cref="Configuration"/>
    /// </summary>
    public static class ConfigurationManager
    {
        private static Configuration _CurrentConfiguration;

        /// <summary>
        /// 获取当前的 SDK 配置
        /// </summary>
        /// <exception cref="ConfigurationException">如果还没进行过 SDK 配置，会抛出异常</exception>
        public static Configuration CurrentConfiguration
        {
            get
            {
                if (_CurrentConfiguration == null)
                {
                    throw new ConfigurationException("SDK 还没有进行配置，请调用 ConfigurationManager.Setup() 方法配置 SDK");
                }
                return _CurrentConfiguration;
            }
        }

        /// <summary>
        /// 进行 SDK 配置
        /// </summary>
        /// <param name="configuration">使用的 SDK 配置实例</param>
        /// <example>
        /// <code language="cs">
        /// var configuration = new Configuration() {
        ///     ServerHost = "199447.qcloud.la",
        ///     AuthServerUrl = "http://10.0.12.135/mina_auth/",
        ///     TunnelServerUrl = "https://ws.qcloud.com/",
        ///     TunnelSignatureKey = "my$ecretkey",
        ///     NetworkTimeout = 30000
        /// };
        /// ConfigurationManager.Setup(configuration);
        /// </code>
        /// </example>
        public static void Setup(Configuration configuration)
        {
            if (configuration == null) throw new ConfigurationException("配置不能为空", new ArgumentNullException(nameof(configuration)));
            if (configuration.ServerHost == null) throw new ConfigurationException("服务器主机配置不能为空", new ArgumentNullException(nameof(configuration.ServerHost)));
            if (configuration.AuthServerUrl == null) throw new ConfigurationException("鉴权服务器配置不能为空", new ArgumentNullException(nameof(configuration.AuthServerUrl)));
            if (configuration.TunnelServerUrl == null) throw new ConfigurationException("信道服务器配置不能为空", new ArgumentNullException(nameof(configuration.TunnelServerUrl)));
            if (configuration.TunnelSignatureKey == null) throw new ConfigurationException("信道服务密钥配置不能为空", new ArgumentNullException(nameof(configuration.TunnelSignatureKey)));
            _CurrentConfiguration = configuration;
        }

        /// <summary>
        /// 从 Json 配置文件对 SDK 进行配置
        /// </summary>
        /// <remarks>
        /// 配置文件应该是一个 JSON 文件，其内容如下：
        /// 
        /// <code language="json">
        /// {
        ///     "serverHost": "199447.qcloud.la",
        ///     "authServerUrl": "http://10.0.12.135/mina_auth/",
        ///     "tunnelServerUrl": "https://ws.qcloud.com/",
        ///     "tunnelSignatureKey: "my$ecretkey",
        ///     "networkTimeout": 30000
        /// }
        /// </code>
        /// 
        /// </remarks>
        /// <param name="configFilePath">配置文件路径</param>
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
