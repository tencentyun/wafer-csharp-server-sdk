using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK.TestServer
{
    public static class JsonExtend
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T ParseFromJson<T>(this T shape, string json)
        {
            return JsonConvert.DeserializeAnonymousType(json, shape);
        }
    }
}
