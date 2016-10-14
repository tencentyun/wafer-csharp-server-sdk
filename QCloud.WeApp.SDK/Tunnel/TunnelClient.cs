using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QCloud.WeApp.SDK
{
    internal static class TunnelClient
    {
        private static string _id = null;
        public static string Id
        {
            get
            {
                if (_id == null)
                {
                    _id = ConfigurationManager.CurrentConfiguration.ServerHost.HashMd5();
                }
                return _id;
            }
        }
        public static string Key
        {
            get
            {
                return ConfigurationManager.CurrentConfiguration.TunnelSignatureKey;
            }
        }
    }
}
