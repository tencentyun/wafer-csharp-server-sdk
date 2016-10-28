using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK.Authorization
{
    /// <summary>
    /// 表示登录服务发生异常
    /// </summary>
    public class LoginServiceException : Exception
    {
        /// <summary>
        /// 异常的类型，可参考 Constants 中的错误常量
        /// </summary>
        /// <seealso cref="Constants"/>
        public string Type { get; private set; }
        internal LoginServiceException(string type, string message) : this(type, message, null) { }
        internal LoginServiceException(string type, string message, Exception innerException) : base(message, innerException)
        {
            this.Type = type;
        }
    }

}
