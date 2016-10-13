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

        public Tunnel RequestConnect(string skey, string receiveUrl)
        {
            var result = Request("/get/wsurl?uin=208852691", new { skey, receiveUrl, protocolType = "wss" });
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
                var result = Request("/ws/push", data);
                return result.code == 0;
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
        public dynamic Request(string path, object data)
        {
            string url = APIEndpoint + path;
            string responseContent;

            try
            {
                string requestContent = BuildRequestContent(data);
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
                dynamic body = JsonConvert.DeserializeObject(responseContent);

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

        public string BuildRequestContent(object data)
        {
            var signature = Signature(data);
            var stringBody = JsonConvert.SerializeObject(new { data, signature });
            return stringBody;
        }

        public string Signature(object data)
        {
            string input = JsonConvert.SerializeObject(data);
            return input.ComputeSignature();
        }
        
    }
}
