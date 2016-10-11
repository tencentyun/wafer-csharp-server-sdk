using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK
{
    internal class TunnelAPI
    {
        /// <summary>
        /// 从配置文件读取 API 访问地址
        /// </summary>
        private string APIEndpoint
        {
            get
            {
                return ConfigurationManager.CurrentConfiguration.TunnelServerUrl;
            }
        }

        public async Task<Tunnel> RequestConnect(string skey, string receiveUrl)
        {
            var result = await Request("/get/wsurl", "RequestConnect", new { skey, receiveUrl });
            return new Tunnel()
            {
                Id = result.tunnelId,
                ConnectUrl = result.connectUrl
            };
        }

        public async Task EmitMessage(string type, object content)
        {

        }

        /// <summary>
        /// 通用 API 请求方法
        /// </summary>
        /// <param name="api">API 名称</param>
        /// <param name="param">API 参数</param>
        /// <returns>API 返回的数据</returns>
        public async Task<dynamic> Request(string path, string api, object param)
        {
            var http = Http.CreateClient();

            HttpResponseMessage response = null;
            try
            {
                response = await http.PostAsync(APIEndpoint + path, BuildRequestBody(api, param));

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpRequestException("错误的 HTTP 响应：" + response.StatusCode);
                }
            }
            catch (Exception error)
            {
                throw new HttpRequestException("请求信道 API 失败，网络异常或鉴权服务器错误", error);
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("==============Response==============");
            Debug.WriteLine(responseBody);
            Debug.WriteLine("");
            try
            {
                dynamic body = JsonConvert.DeserializeObject(responseBody);

                if (body.code != 0)
                {
                    throw new Exception($"信道服务调用失败：#{body.code} - ${body.message}");
                }
                // TODO 校验签名
                return body.data;
            }
            catch (JsonException e)
            {
                throw new JsonException("信道服务器响应格式错误，无法解析 JSON 字符串", e);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public StringContent BuildRequestBody(string api, object param)
        {
            var signature = Signature(api, param);
            var stringBody = JsonConvert.SerializeObject(new { api, param, signature });
            Debug.WriteLine("==============Request==============");
            Debug.WriteLine(stringBody);
            Debug.WriteLine("");
            return new StringContent(stringBody, new UTF8Encoding(false));
        }

        public string Signature(string api, object param)
        {
            string input = JsonConvert.SerializeObject(new { api, param });
            return input.ComputeSignature();
        }
        
    }
}
