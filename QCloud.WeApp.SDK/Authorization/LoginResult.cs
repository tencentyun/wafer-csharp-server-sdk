using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK
{
    public class LoginResult
    {
        public bool Success { get; set; }

        public string Id { get; set; }

        public string Skey { get; set; }

        public UserInfo UserInfo { get; set; }
    }
}
