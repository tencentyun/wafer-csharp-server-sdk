using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK
{
    /// <summary>
    /// 表示对 SDK 进行配置时产生的异常
    /// </summary>
    [Serializable]
    public class ConfigurationException : Exception
    {
        internal ConfigurationException() { }
        internal ConfigurationException(string message) : base(message) { }
        internal ConfigurationException(string message, Exception inner) : base(message, inner) { }
        internal ConfigurationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
