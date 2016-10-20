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
    internal interface IWebRequestProvider
    {
        HttpWebRequest Create(string url);
    }

    internal class DefaultWebRequestProvider: IWebRequestProvider
    {
        public HttpWebRequest Create(string url)
        {
            return WebRequest.CreateHttp(url);
        }
    }

    internal static class Http
    {
        public static void WriteJson(this HttpResponseBase response, object json)
        {
            response.AddHeader("Content-Type", "application/json");
            response.Write(JsonConvert.SerializeObject(json));
        }

        public static IWebRequestProvider WebRequestProvider = new DefaultWebRequestProvider();

        public static string Request(string method, string url, string body)
        {
            var bodyBytes = Encoding.UTF8.GetBytes(body);
            HttpWebRequest request = WebRequestProvider.Create(url);
            request.Timeout = ConfigurationManager.CurrentConfiguration.NetworkTimeout * 1000;
            request.Method = method;
            request.ContentType = "application/json";
            request.ContentLength = bodyBytes.Length;
            request.GetRequestStream().Write(bodyBytes, 0, bodyBytes.Length);

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Server Error: Response with {response.StatusCode}");
            }

            var responseStream = response.GetResponseStream();
            using (StreamReader responseReader = new StreamReader(responseStream))
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
    }
}
