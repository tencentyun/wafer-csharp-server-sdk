using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QCloud.WeApp.SDK
{
    public partial class TunnelService
    {
        /// <summary>
        /// 对信道服务器使用 POST 方法推送到业务服务器的报文进行处理
        /// </summary>
        /// <param name="handler">客户使用的信道服务处理实例，用于处理解析之后的包</param>
        private void HandlePost(ITunnelHandler handler, TunnelHandleOptions options)
        {
            using (SdkDebug.WriteLineAndIndent($"> 收到信道服务器 POST 过来的报文 (Time: {DateTime.Now.ToString("HH:mm:ss")})"))
            {
                DoHandlePost(handler);
            }
        }

        /// <remarks>
        /// 对于推送过来的报文，我们这样进行处理：
        ///     1. 读取报文内容
        ///     2. 解析报文内容成 JSON
        ///     3. 检查报文签名，如果失败，则忽略报文
        ///     4. 解析报文所携带包数据（解析成功直接响应）
        ///     5. 根据包类型（connect/message/close）给到客户指定的处理实例进行处理
        /// </remarks>
        private void DoHandlePost(ITunnelHandler handler)
        {
            #region 1. 读取报文内容
            string requestContent = null;
            try
            {
                using (Stream stream = Request.InputStream)
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        requestContent = reader.ReadToEnd();
                    }
                }
                using (SdkDebug.WriteLineAndIndent("> 读取报文内容：成功"))
                {
                    SdkDebug.WriteLine(requestContent);
                }
            }
            catch (Exception error)
            {
                using (SdkDebug.WriteLineAndIndent("> 读取报文内容：失败"))
                {
                    SdkDebug.WriteLine(error);
                }
                throw new Exception("读取报文失败", error);
            }
            #endregion

            #region 2. 读取报文内容成 JSON 并保存在 body 变量中        
            var body = new
            {
                data = "{encode data}",
                dataEncode = "json",
                signature = string.Empty
            };
            try
            {
                body = JsonConvert.DeserializeAnonymousType(requestContent, body);
                SdkDebug.WriteLine("> 解析报文内容：成功");
            }
            catch (JsonException ex)
            {
                using (SdkDebug.WriteLineAndIndent("> 解析报文内容：失败"))
                {
                    SdkDebug.WriteLine(ex);
                }
                Response.WriteJson(new
                {
                    code = 9001,
                    message = "Cant not parse the request body: invalid json"
                });
                return;
            }
            #endregion

            #region 3. 检查报文签名
            string data = body.data;
            string signature = body.signature;
            string computedSignature = (data + TunnelClient.Key).HashSha1();
            if (computedSignature != signature)
            {
                using (SdkDebug.WriteLineAndIndent("> 检查签名：失败"))
                {
                    SdkDebug.WriteLine($"报文签名：{signature}，计算结果：{computedSignature}，tcKey: {TunnelClient.Key}");
                }
                Response.WriteJson(new
                {
                    code = 9003,
                    message = "Bad Request - 签名错误"
                });
                return;
            }
            using (SdkDebug.WriteLineAndIndent("> 检查签名：成功"))
            {
                SdkDebug.WriteLine(signature);
            }
            #endregion

            #region 4. 解析报文中携带的包数据
            var packet = new
            {
                tunnelId = string.Empty,
                type = string.Empty,
                content = string.Empty
            };
            try
            {
                packet = JsonConvert.DeserializeAnonymousType(data, packet);
            }
            catch (JsonException ex)
            {
                using (SdkDebug.WriteLineAndIndent("> 解析包数据：失败"))
                {
                    SdkDebug.WriteLine(ex);
                }
                Response.WriteJson(new
                {
                    code = 9004,
                    message = "Bad Request - 无法解析的数据包"
                });
                return;
            }
            using (SdkDebug.WriteLineAndIndent("> 解析包数据：成功"))
            {
                SdkDebug.WriteLine($"tunnelId = {packet.tunnelId}");
                SdkDebug.WriteLine($"type = {packet.type}");
                SdkDebug.WriteLine($"content = {packet.content}");
            }
            Response.WriteJson(new
            {
                code = 0,
                message = "OK"
            });
            #endregion

            #region 5. 交给客户处理实例处理报文
            var tunnel = Tunnel.GetById(packet.tunnelId);
            try
            {
                using (SdkDebug.WriteLineAndIndent("> 处理数据包：开始"))
                {
                    switch (packet.type)
                    {
                        case "connect":
                            handler.OnTunnelConnect(tunnel);
                            break;
                        case "message":
                            handler.OnTunnelMessage(tunnel, new TunnelMessage(packet.content));
                            break;
                        case "close":
                            handler.OnTunnelClose(tunnel);
                            break;
                    }
                }
                SdkDebug.WriteLine("> 处理数据包：完成");
            }
            catch (Exception ex)
            {
                using (SdkDebug.WriteLineAndIndent("> 处理数据包：发生异常"))
                {
                    SdkDebug.WriteLine(ex);
                }
            }
            #endregion
        }
    }
}
