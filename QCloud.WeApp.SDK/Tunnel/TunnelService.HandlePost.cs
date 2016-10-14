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
        private void HandlePost(ITunnelHandler handler, TunnelHandleOptions options)
        {
            string requestBody = null;

            using (Stream stream = Request.InputStream)
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    requestBody = reader.ReadToEnd();
                }
            }
            
            try
            {
                var bodyDefination = new {
                    data = "{encode data}",
                    dataEncode = "json",
                    signature = string.Empty
                };

                var body = JsonConvert.DeserializeAnonymousType(requestBody, bodyDefination);
                var data = body.data;
                var signature = body.signature;
                if ((data + TunnelClient.Key).HashSha1() != signature)
                {
                    Response.WriteJson(new
                    {
                        code = 9003,
                        message = "Bad Request - 签名错误"
                    });
                    return;
                }

                var packetShape = new
                {
                    tunnelId = string.Empty,
                    type = string.Empty,
                    content = string.Empty
                };

                var packet = JsonConvert.DeserializeAnonymousType(data, packetShape);                
                
                Response.WriteJson(new
                {
                    code = 0,
                    message = "OK"
                });

                var tunnel = Tunnel.GetById(packet.tunnelId);
                try
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
                catch(Exception e) {
                    // ignore
                }
            }
            catch (JsonException)
            {
                Response.WriteJson(new
                {
                    code = 9001,
                    message = "Cant not parse the request body: invalid json"
                });
                return;
            }
            catch (Exception)
            {
                Response.WriteJson(new
                {
                    code = 10001,
                    message = "Unexpected Error"
                });
                return;
            }

        }

        private void LogRequest(string requestBody, string handleResult)
        {
            bool log = true;
            if (log)
            {
                try
                {
                    File.WriteAllText($"C:\\requests\\{DateTime.Now.ToString("yyyyMMdd_HH_mm_ss")}", requestBody + "\n\n" + handleResult);
                }
                catch { }
            }
        }
    }
}
