using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QCloud.WeApp.SDK;
using Moq;
using QCloud.WeApp.SDK.Tunnel;

namespace QCloud.WeApp.SDK.Tests.Tunnel
{
    public partial class TunnelServiceTest
    {
        [TestMethod]
        public void TestGetConnection()
        {
            HttpMock httpMock = helper.CreateTunnelHttpMock("GET", true);
            TunnelService tunnelService = new TunnelService(httpMock.Request.Object, httpMock.Response.Object);
            var handlerMock = new Mock<ITunnelHandler>();
            handlerMock.Setup(x => x.OnTunnelConnect(null));
            tunnelService.Handle(handlerMock.Object, new TunnelHandleOptions() { CheckLogin = false });
        }
    }
}
