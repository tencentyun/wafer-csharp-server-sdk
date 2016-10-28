using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK.Tunnel
{
    /// <summary>
    /// 表示发送消息到信道服务器时发生的异常
    /// </summary>
    [Serializable]
    public class EmitException : Exception
    {
        internal EmitException() { }
        internal EmitException(string message) : base(message) { }
        internal EmitException(string message, Exception inner) : base(message, inner) { }
        internal protected EmitException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
