using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.IO;

namespace QCloud.WeApp.SDK
{
    internal static class Http
    {
        public static void WriteJson(this HttpResponseBase response, object json)
        {
            response.AddHeader("Content-Type", "application/json");
            response.Write(JsonConvert.SerializeObject(json));
        }

        public static string Request(string method, string url, string body)
        {
            var bodyBytes = Encoding.UTF8.GetBytes(body);
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = method;
            request.ContentType = "application/json";
            request.ContentLength = bodyBytes.Length;
            request.GetRequestStream().Write(bodyBytes, 0, bodyBytes.Length);

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Server Error: Response with {response.StatusCode}");
            }

            using (StreamReader responseReader = new StreamReader(response.GetResponseStream()))
            {
                return responseReader.ReadToEnd();
            }
        }

        public static string Post(string url, string body)
        {
            return Request("POST", url, body);
        }

        public static string Get(string url, string body)
        {
            return Request("GET", url, body);
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
