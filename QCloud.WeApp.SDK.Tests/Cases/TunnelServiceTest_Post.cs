using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using QCloud.WeApp.SDK;
using QCloud.WeApp.SDK.Authorization;
using QCloud.WeApp.SDK.Tunnel;
using System;
using System.IO;
using System.Web;

namespace QCloud.WeApp.Tests
{
    public partial class TunnelServiceTest
    {
        [TestMethod]
        [Description("测试处理信道服务器发送的包（类型：连接建立）")]
        public void TestPostConnectPacket()
        {
            var httpContextMock = helper.CreateTunnelHttpContextMock("POST");
            helper.SetupRequestBody(httpContextMock, helper.BuildPacket(new
            {
                type = "connect",
                tunnelId = "tunnel1"
            }.ToJson()));

            var tunnelService = new TunnelService(httpContextMock.Object.Request, httpContextMock.Object.Response);
            var tunnelHandlerMock = new Mock<ITunnelHandler>();
            tunnelService.Handle(tunnelHandlerMock.Object);

            tunnelHandlerMock.Verify(
                expression: x => x.OnTunnelConnect(It.Is((SDK.Tunnel.Tunnel tunnel) => tunnel.Id == "tunnel1")),
                times: Times.Once(),
                failMessage: "接收到连接数据包之后没有正确调用 OnTunnelConnect 事件"
            );
            helper.VerifyHandleResponseSuccess(httpContextMock);
        }

        [TestMethod]
        [Description("测试处理信道服务器发送的包（类型：连接关闭）")]
        public void TestPostClosePacket()
        {
            var httpContextMock = helper.CreateTunnelHttpContextMock("POST");
            helper.SetupRequestBody(httpContextMock, helper.BuildPacket(new
            {
                type = "close",
                tunnelId = "tunnel1"
            }.ToJson()));

            var tunnelService = new TunnelService(httpContextMock.Object.Request, httpContextMock.Object.Response);
            var tunnelHandlerMock = new Mock<ITunnelHandler>();
            tunnelService.Handle(tunnelHandlerMock.Object);

            tunnelHandlerMock.Verify(
                expression: x => x.OnTunnelClose(It.Is((SDK.Tunnel.Tunnel tunnel) => tunnel.Id == "tunnel1")),
                times: Times.Once(),
                failMessage: "接收到关闭数据包之后没有正确调用 OnTunnelClose 事件"
            );
            helper.VerifyHandleResponseSuccess(httpContextMock);
        }

        [TestMethod]
        [Description("测试处理信道服务器发送的包（类型：接收消息）")]
        public void TestPostMessagePacket()
        {
            var httpContextMock = helper.CreateTunnelHttpContextMock("POST");
            helper.SetupRequestBody(httpContextMock, helper.BuildPacket(new
            {
                type = "message",
                tunnelId = "tunnel1",
                content = new {
                    type = "test-type",
                    content = "test-content"
                }.ToJson()
            }.ToJson()));

            var tunnelService = new TunnelService(httpContextMock.Object.Request, httpContextMock.Object.Response);
            var tunnelHandlerMock = new Mock<ITunnelHandler>();
            tunnelService.Handle(tunnelHandlerMock.Object);

            Func<TunnelMessage, bool> IsValidMessage = message =>
            {
                return message.Type == "test-type" && message.Content == "test-content";
            };

            tunnelHandlerMock.Verify(
                expression: x => x.OnTunnelMessage(
                    It.Is((Tunnel tunnel) => tunnel.Id == "tunnel1"),
                    It.Is((TunnelMessage message) => IsValidMessage(message))
                ),
                times: Times.Once(),
                failMessage: "接收到关闭数据包之后没有正确调用 OnTunnelClose 事件"
            );
            helper.VerifyHandleResponseSuccess(httpContextMock);
        }

        [TestMethod]
        [Description("测试处理信道服务器发送的包（类型：无法解析的消息）")]
        public void TestPostUnknowMessagePacket()
        {
            var httpContextMock = helper.CreateTunnelHttpContextMock("POST");
            helper.SetupRequestBody(httpContextMock, helper.BuildPacket(new
            {
                type = "message",
                tunnelId = "tunnel1",
                content = "unknown-raw"
            }.ToJson()));

            var tunnelService = new TunnelService(httpContextMock.Object.Request, httpContextMock.Object.Response);
            var tunnelHandlerMock = new Mock<ITunnelHandler>();
            tunnelService.Handle(tunnelHandlerMock.Object);

            Func<TunnelMessage, bool> IsValidMessage = message =>
            {
                return message.Type == "UnknownRaw" && message.Content == "unknown-raw";
            };

            tunnelHandlerMock.Verify(
                expression: x => x.OnTunnelMessage(
                    It.Is((Tunnel tunnel) => tunnel.Id == "tunnel1"),
                    It.Is((TunnelMessage message) => IsValidMessage(message))
                ),
                times: Times.Once(),
                failMessage: "接收到关闭数据包之后没有正确调用 OnTunnelClose 事件"
            );
            helper.VerifyHandleResponseSuccess(httpContextMock);
        }

        [TestMethod]
        [Description("测试处理信道服务器发送的未知包")]
        public void TestPostUnknowPacket()
        {
            var httpContextMock = helper.CreateTunnelHttpContextMock("POST");
            helper.SetupRequestBody(httpContextMock, helper.BuildPacket(new
            {
                type = "unknown",
                tunnelId = "tunnel1"
            }.ToJson()));

            var tunnelService = new TunnelService(httpContextMock.Object.Request, httpContextMock.Object.Response);
            var tunnelHandlerMock = new Mock<ITunnelHandler>();
            tunnelService.Handle(tunnelHandlerMock.Object);
            
            helper.CheckNoMethodIsCalled(tunnelHandlerMock);
            helper.VerifyHandleResponseSuccess(httpContextMock);
        }


        [TestMethod]
        [Description("测试处理信道服务器发送的签名错误的包")]
        public void TestPostSignatureErrorPacket()
        {
            var httpContextMock = helper.CreateTunnelHttpContextMock("POST");
            helper.SetupRequestBody(httpContextMock, helper.BuildPacket(new
            {
                type = "connect",
                tunnelId = "tunnel1"
            }.ToJson(), fakeSignature: true));

            var tunnelService = new TunnelService(httpContextMock.Object.Request, httpContextMock.Object.Response);
            var tunnelHandlerMock = new Mock<ITunnelHandler>();
            tunnelService.Handle(tunnelHandlerMock.Object);

            helper.CheckNoMethodIsCalled(tunnelHandlerMock);
            helper.VerifyHandleResponseFail(httpContextMock);
        }

        [TestMethod]
        [Description("测试处理信道服务器发送的错误包内容")]
        public void TestPostBadPacket()
        {
            var httpContextMock = helper.CreateTunnelHttpContextMock("POST");
            helper.SetupRequestBody(httpContextMock, helper.BuildPacket("illgal packet"));

            var tunnelService = new TunnelService(httpContextMock.Object.Request, httpContextMock.Object.Response);
            var tunnelHandlerMock = new Mock<ITunnelHandler>();
            tunnelService.Handle(tunnelHandlerMock.Object);

            helper.CheckNoMethodIsCalled(tunnelHandlerMock);
            helper.VerifyHandleResponseFail(httpContextMock);
        }

        
        [TestMethod]
        [Description("测试处理信道服务器发送的错误报文")]
        public void TestPostBadRequest()
        {
            var httpContextMock = helper.CreateTunnelHttpContextMock("POST");
            helper.SetupRequestBody(httpContextMock, "illegal request");

            var tunnelService = new TunnelService(httpContextMock.Object.Request, httpContextMock.Object.Response);
            var tunnelHandlerMock = new Mock<ITunnelHandler>();
            tunnelService.Handle(tunnelHandlerMock.Object);

            helper.CheckNoMethodIsCalled(tunnelHandlerMock);
            helper.VerifyHandleResponseFail(httpContextMock);
        }

        [TestMethod]
        [Description("信道服务器报文读取出错的情况")]
        public void TestPostBadNetwork()
        {
            var httpContextMock = helper.CreateTunnelHttpContextMock("POST");
            httpContextMock.Setup(x => x.Request.InputStream).Throws(new HttpException("Bad Network"));

            var tunnelService = new TunnelService(httpContextMock.Object.Request, httpContextMock.Object.Response);
            var tunnelHandlerMock = new Mock<ITunnelHandler>();

            Exception errorShouldThrow = null;
            try
            {
                tunnelService.Handle(tunnelHandlerMock.Object);
            } catch (Exception error)
            {
                errorShouldThrow = error;
            }

            helper.CheckNoMethodIsCalled(tunnelHandlerMock);
            Assert.IsNotNull(errorShouldThrow);
        }

    }
}
