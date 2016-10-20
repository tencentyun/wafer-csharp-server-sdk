using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QCloud.WeApp.SDK;
using QCloud.WeApp.SDK.Authorization;
using QCloud.WeApp.SDK.Tunnel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QCloud.WeApp.Tests
{
    class TunnelServiceTestHelper
    {
        public Mock<HttpContextBase> CreateTunnelHttpContextMock(string method, string sessionType = null)
        {
            var mock = new Mock<HttpContextBase>();
            
            mock.Setup(x => x.Request.Url).Returns(new Uri("http://test.qcloud.la/tunnel"));
            mock.Setup(x => x.Request.HttpMethod).Returns(method.ToUpper());
            mock.Setup(x => x.Request.Headers).Returns(() =>
            {
                var headers = new NameValueCollection();
                if (sessionType == "valid")
                {
                    headers.Add("X-WX-Id", "valid-id");
                    headers.Add("X-WX-Skey", "valid-key");
                }
                else if (sessionType == "invalid")
                {
                    headers.Add("X-WX-Id", "invalid-id");
                    headers.Add("X-WX-Skey", "invalid-key");
                }
                return headers;
            });
            mock.Setup(x => x.Response.Write(null));
            return mock;
        }

        public void SetupRequestBody(Mock<HttpContextBase> mock, string body)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms, Encoding.UTF8);
            mock.Setup(x => x.Request.InputStream).Returns(ms);
            sw.Write(body);
            sw.Flush();
            ms.Position = 0;
        }

        public bool CheckBodyHasMagicId(JObject body)
        {
            return body["F2C224D4-2BCE-4C64-AF9F-A6D872000D1A"].Value<int>() == 1;
        }

        public bool CheckBodyHasUrl(string content)
        {
            return new { url = string.Empty }.ParseFromJson(content).url != null;
        }

        public void VerifyHandleResponseSuccess(Mock<HttpContextBase> mock)
        {
            mock.Verify(
                expression: x => x.Response.Write(It.Is<string>(
                    content => new { code = 0 }.ParseFromJson(content).code == 0)
                ),
                times: Times.Once(),
                failMessage: "没有给到信道服务器正确的响应"
            );
        }

        
        public void VerifyHandleResponseFail(Mock<HttpContextBase> mock)
        {
            mock.Verify(
                expression: x => x.Response.Write(It.Is<string>(
                    content => new { code = 0 }.ParseFromJson(content).code != 0)
                ),
                times: Times.Once(),
                failMessage: "没有给到信道服务器合适的错误响应"
            );
        }

        public string BuildPacket(string data, bool fakeSignature = false)
        {
            return new
            {
                data = data,
                dataEncode = "json",
                signature = fakeSignature ? "fake-signature" : (data + ConfigurationManager.CurrentConfiguration.TunnelSignatureKey).HashSha1()
            }.ToJson();
        }

        public class SdkWebRequestProviderMock: Mock<IWebRequestProvider>, IDisposable
        {
            private IWebRequestProvider originProvider;
            private MemoryStream requestStream = new MemoryStream();

            public SdkWebRequestProviderMock()
            {
                originProvider = Http.WebRequestProvider;
                Http.WebRequestProvider = Object;
            }

            public void SetupResponseBody(string body)
            {
                MemoryStream responseStream = new MemoryStream();
                StreamWriter responseWriter = new StreamWriter(responseStream, Encoding.UTF8);
                Mock<HttpWebResponse> responseMock = new Mock<HttpWebResponse>();

                responseWriter.Write(body);
                responseWriter.Flush();
                responseStream.Position = 0;

                responseMock.Setup(x => x.GetResponseStream()).Returns(responseStream);
                responseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
                Setup(x => x.Create(null).GetResponse()).Returns(responseMock.Object);
                Setup(x => x.Create(null)
                    .GetRequestStream()
                    .Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Callback((byte[] bytes, int offset, int count) => {
                        requestStream.Write(bytes, offset, count);
                    });
            }

            private string responseContent;
            public string GetRequestContent()
            {
                if (responseContent != null)
                {
                    return responseContent;
                }
                requestStream.Position = 0;
                using(StreamReader requesetReader = new StreamReader(requestStream, Encoding.UTF8))
                {
                    return responseContent = requesetReader.ReadToEnd();
                }
            }

            public void Dispose()
            {
                Http.WebRequestProvider = originProvider;
            }
        }

        public SdkWebRequestProviderMock CreateWebRequestProviderMock()
        {
            return new SdkWebRequestProviderMock();
        }


        public class PacketDiliver
        {
            public string[] tunnelIds;
            public string type;
            public string content;
        }
        
        public PacketDiliver[] ResolvePackets(string data)
        {
            var decodeData = new
            {
                data = "",
                dataEncode = "json",
                signature = ""
            }.ParseFromJson(data);
            var packets = JsonConvert.DeserializeObject<PacketDiliver[]>(decodeData.data);
            return packets;
        }

        public void CheckNoMethodIsCalled(Mock<ITunnelHandler> mock)
        {
            mock.Verify(
                expression: x => x.OnTunnelRequest(It.IsAny<Tunnel>(), It.IsAny<UserInfo>()),
                times: Times.Never(),
                failMessage: $"错误调用 OnTunnelRequest() 方法"
            );

            mock.Verify(
                expression: x => x.OnTunnelConnect(It.IsAny<Tunnel>()),
                times: Times.Never(),
                failMessage: "错误调用 OnTunnelConnect() 方法"
            );

            mock.Verify(
                expression: x => x.OnTunnelMessage(It.IsAny<Tunnel>(), It.IsAny<TunnelMessage>()),
                times: Times.Never(),
                failMessage: "错误调用 OnTunnelMessage() 方法"
            );

            mock.Verify(
                expression: x => x.OnTunnelClose(It.IsAny<Tunnel>()),
                times: Times.Never(),
                failMessage: "错误调用 OnTunnelClose() 方法"
            );
        }
    }
}
