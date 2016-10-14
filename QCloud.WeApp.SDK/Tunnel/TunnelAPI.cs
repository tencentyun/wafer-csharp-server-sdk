using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QCloud.WeApp.SDK
{
    internal class TunnelAPI
    {
        
        /// <summary>
        /// 从配置文件读取 API 访问地址
        /// </summary>
        private string TunnelServerUrl
        {
            get
            {
                return ConfigurationManager.CurrentConfiguration.TunnelServerUrl;
            }
        }

        public Tunnel RequestConnect(string skey, string receiveUrl)
        {
            var result = Request("/get/wsurl", new { skey, receiveUrl, protocolType = "wss" }, true);
            string tunnelId = result.tunnelId;
            string connectUrl = result.connectUrl;

            return new Tunnel()
            {
                Id = tunnelId,
                ConnectUrl = connectUrl
            };
        }

        public bool EmitMessage(IEnumerable<string> tunnelIds, string messageType, object messageContent = null)
        {
            return EmitPacket(tunnelIds, "message", new { type = messageType, content = messageContent });
        }

        public bool EmitPacket(IEnumerable<string> tunnelIds, string packetType, object packetContent)
        {
            if (tunnelIds.Count() == 0) return true;
            var data = new object[] {
                new {
                    type = packetType,
                    tunnelIds,
                    content = packetContent == null ? null : JsonConvert.SerializeObject(packetContent)
                }
            };
            try
            {
                Request("/ws/push", data);
                return true;
            }
            catch (Exception e) {
                Debug.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// 通用 API 请求方法
        /// </summary>
        /// <param name="api">API 名称</param>
        /// <param name="data">API 参数</param>
        /// <returns>API 返回的数据</returns>
        public dynamic Request(string path, object data, bool emitSkey = false)
        {
            string url = TunnelServerUrl + path;
            string responseContent;

            try
            {
                string requestContent = BuildRequestContent(data, emitSkey);
                Debug.WriteLine("==============Tunnel Request==============");
                Debug.WriteLine(requestContent);
                Debug.WriteLine("");

                responseContent = Http.Post(url, requestContent);

                Debug.WriteLine("==============Tunnel Response==============");
                Debug.WriteLine(responseContent);
                Debug.WriteLine("");
            }
            catch (Exception httpException)
            {
                throw new HttpRequestException("请求信道 API 失败，网络异常或鉴权服务器错误", httpException);
            }
            try
            {
                var bodyShape = new
                {
                    code = 0,
                    message = "OK",
                    data = "{}"
                };
                var body = JsonConvert.DeserializeAnonymousType(responseContent, bodyShape, new JsonSerializerSettings() {
                    NullValueHandling = NullValueHandling.Include
                });

                if (body.code != 0)
                {
                    throw new Exception($"信道服务调用失败：#{body.code} - ${body.message}");
                }
                return body.data == null ? null : JsonConvert.DeserializeObject(body.data);
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

        public string BuildRequestContent(object data, bool includeSkey)
        {
            var encodedData = JsonConvert.SerializeObject(data);
            var requestPayload = new
            {
                data = encodedData,
                dataEncode = "json",
                tcId = TunnelClient.Id,
                tcKey = includeSkey ? TunnelClient.Key : null,
                signature = Signature(encodedData)
            };
            var stringBody = JsonConvert.SerializeObject(requestPayload, new JsonSerializerSettings() {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });
            return stringBody;
        }
        
        private string Signature(string data)
        {
            return (data + TunnelClient.Key).HashSha1();
        }
    }
}
