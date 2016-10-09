using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK
{
    public class LoginServiceException : Exception
    {
        public string Type { get; private set; }
        public LoginServiceException(string type, string message) : this(type, message, null) { }
        public LoginServiceException(string type, string message, Exception innerException) : base(message, innerException)
        {
            this.Type = type;
        }
    }

}
