using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace QCloud.WeApp.SDK
{
    internal class TunnelAPI
    {

        /// <summary>
        /// 读取信道服务器地址
        /// </summary>
        private string TunnelServerUrl
        {
            get
            {
                return ConfigurationManager.CurrentConfiguration.TunnelServerUrl;
            }
        }

        /// <summary>
        /// API：申请信道连接地址
        /// <param name="receiveUrl">注册的消息推送地址，申请到的信道连接后，消息将推送到这个地址上</param>
        /// </summary>
        public Tunnel RequestConnect(string receiveUrl)
        {
            var result = Request("/get/wsurl", new { skey = TunnelClient.Key, receiveUrl, protocolType = "wss" }, true);
            string tunnelId = result.tunnelId;
            string connectUrl = result.connectUrl;

            return new Tunnel(tunnelId)
            {
                ConnectUrl = connectUrl
            };
        }

        public EmitResult EmitMessage(IEnumerable<string> tunnelIds, string messageType, object messageContent = null)
        {
            using (SdkDebug.WriteLineAndIndent("> 发送消息"))
            {
                SdkDebug.WriteLine($"> 消息类型：{messageType}");
                return EmitPacket(tunnelIds, "message", new { type = messageType, content = messageContent });
            }
        }

        /// <summary>
        /// 发送数据包到信道服务器
        /// </summary>
        /// <param name="tunnelIds">指定数据包需要送到的信道 ID 列表</param>
        /// <param name="packetType">指定数据包的类型（message/close）</param>
        /// <param name="packetContent">指定数据包的内容</param>
        /// <returns>返回数据包发送结果，可能会包含不可用信道的列表</returns>
        /// <exception cref="EmitException">网络不可用或者信道服务器不可用</exception>
        public EmitResult EmitPacket(IEnumerable<string> tunnelIds, string packetType, object packetContent)
        {
            if (tunnelIds.Count() == 0) return new EmitResult()
            {
                TunnelIvalidInfos = new List<TunnelInvalidInfo>()
            };
            var data = new object[] {
                new {
                    type = packetType,
                    tunnelIds,
                    content = packetContent == null ? null : JsonConvert.SerializeObject(packetContent)
                }
            };
            using (SdkDebug.WriteLineAndIndent("> 发送数据包"))
            {
                SdkDebug.WriteLine($"> 信道 ID 列表：[{tunnelIds}]");
                SdkDebug.WriteLine($"> 包类型: {packetType}");
                try
                {
                    var emitResult = Request("/ws/push", data);
                    IEnumerable<string> invalidTunnels = emitResult.invalidTunnels;
                    return new EmitResult()
                    {
                        TunnelIvalidInfos = invalidTunnels.Select(tunnelId => new TunnelInvalidInfo()
                        {
                            TunnelId = tunnelId,
                            Type = TunnelInvalidType.TunnelHasClosed
                        })
                    };
                }
                catch (Exception error)
                {
                    throw new EmitException("网络不可用或者信道服务器不可用", error);
                }
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
            using (SdkDebug.WriteLineAndIndent("> 请求信道服务 API"))
            {
                return DoRequest(path, data, emitSkey);
            }
        }

        private dynamic DoRequest(string path, object data, bool emitSkey)
        {
            string url = TunnelServerUrl + path;
            string responseContent;

            // 请求信道服务器，获取返回报文
            try
            {
                string requestContent = BuildRequestContent(data, emitSkey);
                DateTime start = DateTime.Now;

                responseContent = Http.Post(url, requestContent);
                DateTime end = DateTime.Now;
                TimeSpan cost = end - start;

                using (SdkDebug.WriteLineAndIndent($"POST {url} (Time: {start.ToString("HH:mm:ss")}, Cost: {cost.TotalMilliseconds}ms)"))
                {
                    using (SdkDebug.WriteLineAndIndent("Requset:"))
                    {
                        SdkDebug.WriteLine(requestContent);
                    }
                    using (SdkDebug.WriteLineAndIndent("Response:"))
                    {
                        SdkDebug.WriteLine(responseContent);
                    }
                }
            }
            catch (Exception error)
            {
                using (SdkDebug.WriteLineAndIndent($"POST {url} (ERROR)"))
                {
                    SdkDebug.WriteLine(error);
                }
                throw new HttpRequestException("请求信道 API 失败，网络异常或鉴权服务器错误", error);
            }

            // 解析返回报文
            try
            {
                var bodyShape = new
                {
                    code = 0,
                    message = "OK",
                    data = "{}"
                };
                var body = JsonConvert.DeserializeAnonymousType(responseContent, bodyShape, new JsonSerializerSettings()
                {
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

        public string BuildRequestContent(object data, bool includeTckey)
        {
            string encodedData = JsonConvert.SerializeObject(data);
            var requestPayload = new
            {
                data = encodedData,
                dataEncode = "json",
                tcId = TunnelClient.Id,
                tcKey = includeTckey ? TunnelClient.Key : null,
                signature = Signature(encodedData)
            };
            string stringBody = JsonConvert.SerializeObject(requestPayload, new JsonSerializerSettings()
            {
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
