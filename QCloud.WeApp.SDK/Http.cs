using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;

namespace QCloud.WeApp.SDK
{
    internal static class Http
    {
        public static void WriteJson(this HttpResponseBase response, object json)
        {
            response.AddHeader("Content-Type", "application/json");
            response.Write(JsonConvert.SerializeObject(json));
        }

        public static HttpClient CreateClient()
        {
            Configuration config = ConfigurationManager.CurrentConfiguration;
            
            if (config.NetworkProxy != null)
            {
                return new HttpClient(new HttpClientHandler()
                {
                    Proxy = new WebProxy(config.NetworkProxy)
                });
            }
            else
            {
                return new HttpClient();
            }
        }
    }
}
