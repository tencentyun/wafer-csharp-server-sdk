using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK
{
    /// <summary>
    /// 表示发送消息到信道服务器时发生的异常
    /// </summary>
    [Serializable]
    public class EmitException : Exception
    {
        public EmitException() { }
        public EmitException(string message) : base(message) { }
        public EmitException(string message, Exception inner) : base(message, inner) { }
        protected EmitException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
