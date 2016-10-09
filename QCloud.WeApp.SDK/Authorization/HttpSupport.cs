using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace QCloud.WeApp.SDK
{
    static class HttpSupport
    {
        public static void WriteJson(this HttpResponseBase response, object json)
        {
            response.AddHeader("Content-Type", "application/json");
            response.Write(JsonConvert.SerializeObject(json));
        }
    }
}
