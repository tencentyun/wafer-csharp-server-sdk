using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;

namespace QCloud.WeApp.TestServer
{
    public class HttpHandler
    {
        private HttpListenerRequest request;
        private HttpListenerResponse response;   
        private string body;
        private string url;

        public HttpHandler(HttpListenerRequest request, HttpListenerResponse response)
        {
            this.request = request;
            this.response = response;
            url = request.RawUrl;
            if (request.HasEntityBody)
            {
                body = request.GetRawEntityBody();
            } else
            {
                body = null;
            }
        }

        public HandleResult Handle()
        {

            switch (url.ToLower())
            {
                case "/":
                    return Output($"SDK Test Server/1.0, Welcome");
                case "/auth":
                    return HandleAuth();
                case "/tunnel/get/wsurl":
                    return HandleTunnelGetUrl();
                case "/tunnel/ws/push":
                    return HandleTunnelPush();
                default:
                    return Output($"Can Not Process {url}");
            }
            
        }

        private HandleResult HandleAuth()
        {
            var packet = new
            {
                @version = 1,
                @componentName = "MA",
                @interface = new {
                    @interfaceName = "",
                    @para = new {
                        code = "",
                        encrypt_data = "",
                        iv = "",
                        id = "",
                        skey = ""
                    }
                }
            };
            try
            {
                packet = packet.ParseFromJson(body);
            }
            catch (Exception)
            {
                return Output("无法解析的 JSON 包");
            }
            switch(packet.@interface.interfaceName)
            {
                case "qcloud.cam.id_skey":
                    return HandleLoginRequest(packet.@interface.para.code, packet.@interface.para.encrypt_data, packet.@interface.para.iv);
                case "qcloud.cam.auth":
                    return HandleCheckRequest(packet.@interface.para.id, packet.@interface.para.skey);
                default:
                    return Output(new {
                        returnCode = -1,
                        message = $"不支持的接口：{packet.@interface.interfaceName}"
                    }.ToJson());
            }
        }

        private HandleResult HandleLoginRequest(string code, string encryptedData, string iv)
        {
            var errorOutput = CommonErrorOutput(code);
            if (errorOutput != null) return errorOutput;

            if (code == "valid-code" && encryptedData == "valid-data" && iv == "valid-iv")
            {
                return Output(new {
                    returnCode = 0,
                    returnMessage = "OK",
                    returnData = new {
                        id = "success_id",
                        skey = "success_skey",
                        user_info = new { nickName = "fake_user", gender = 0 }
                    }
                }.ToJson());
            }
            return Output(new
            {
                returnCode = -1,
                returnMessage = "invalid code or encryptData"
            }.ToJson()); ;
        }
       

        private HandleResult HandleCheckRequest(string id, string skey)
        {
            var errorOutput = CommonErrorOutput(id);
            if (errorOutput != null) return errorOutput;
            if (id == "valid-id" && skey == "valid-key")
            {
                return Output(new
                {
                    returnCode = 0,
                    returnMessage = "OK",
                    returnData = new
                    {
                        id = "success_id",
                        skey = "success_skey",
                        user_info = new { nickName = "fake_user", gender = 0 }
                    }
                }.ToJson());
            }
            if (id == "expect-60011")
            {
                return Output(new
                {
                    returnCode = 60011,
                    returnMessage = "ERR_60011"
                }.ToJson());
            }
            if (id == "expect-60012")
            {
                return Output(new
                {
                    returnCode = 60012,
                    returnMessage = "ERR_60012"
                }.ToJson());
            }
            return Output(new
            {
                returnCode = -1,
                returnMessage = "invalid id or skey"
            }.ToJson()); ;
        }

        private HandleResult HandleTunnelGetUrl()
        {
            var packet = new
            {
                data = "",
                dataEncode = "json",
                tcId = "",
                tcKey = "",
                signature = ""
            };
            try
            {
                packet = packet.ParseFromJson(body);
            }
            catch (JsonException)
            {
                Output("无法解析的 JSON 包");
            }
            var data = new
            {
                receiveUrl = ""
            };
            data = data.ParseFromJson(packet.data);
            var errorOutput = CommonErrorOutput(data.receiveUrl);
            if (errorOutput != null) return errorOutput;

            return Output(new {
                code = 0,
                message = "OK",
                data = new {
                    tunnelId = "tunnel1",
                    connectUrl = "wss://ws.qcloud.com/ws/tunnel1"
                }.ToJson()
            }.ToJson());
        }

        private HandleResult HandleTunnelPush()
        {
            throw new NotImplementedException();
        }

        private HandleResult Output(string content, int delay = 0)
        {
            HandleResult result = new HandleResult()
            {
                Time = DateTime.Now,
                Url = url,
                Request = body,
                Response = content,
                Status = response.StatusCode,
                Method = request.HttpMethod
            };
            if (delay > 0)
            {
                Thread.Sleep(delay * 1000);
            }
            else
            {
                response.Write(content);
            }
            return result;
        }



        private HandleResult CommonErrorOutput(string indicator)
        {
            if (indicator == "expect-500")
            {
                response.StatusCode = 500;
                return Output("Fake Server Error");
            }
            if (indicator == "expect-invalid-json")
            {
                return Output("{invalidJson}");
            }
            if (indicator == "expect-timeout")
            {
                return Output("Timeout", 60);
            }
            return null;
        }
    }
}
