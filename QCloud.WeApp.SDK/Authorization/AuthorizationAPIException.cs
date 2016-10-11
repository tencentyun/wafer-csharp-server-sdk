using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK
{

    [Serializable]
    internal class AuthorizationAPIException : Exception
    {
        public int Code { get; set; }

        public AuthorizationAPIException() { }
        public AuthorizationAPIException(string message) : base(message) { }
        public AuthorizationAPIException(string message, Exception inner) : base(message, inner) { }
        protected AuthorizationAPIException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
