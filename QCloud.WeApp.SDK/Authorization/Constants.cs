using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QCloud.WeApp.SDK.Authorization
{
    /// <summary>
    /// 包含登录服务使用到的常量
    /// </summary>
    public static class Constants
    {
        internal const string WX_SESSION_MAGIC_ID = "F2C224D4-2BCE-4C64-AF9F-A6D872000D1A";
        internal const string WX_HEADER_CODE = "X-WX-Code";
        internal const string WX_HEADER_ID = "X-WX-Id";
        internal const string WX_HEADER_SKEY = "X-WX-Skey";
        internal const string WX_HEADER_ENCRYPT_DATA = "X-WX-Encrypt-Data";

        /// <summary>
        /// 表示请求无效
        /// </summary>
        public const string ERR_INVALID_REQUEST = "ERR_INVALID_REQUEST";

        /// <summary>
        /// 表示登录失败
        /// </summary>
        public const string ERR_LOGIN_FAILED = "ERR_LOGIN_FAILED";

        /// <summary>
        /// 表示会话过期的错误
        /// </summary>
        public const string ERR_INVALID_SESSION = "ERR_INVALID_SESSION";

        /// <summary>
        /// 表示检查登录态失败
        /// </summary>
        public const string ERR_CHECK_LOGIN_FAILED = "ERR_CHECK_LOGIN_FAILED";
    }
}
