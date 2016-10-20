using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using QCloud.WeApp.SDK;
using QCloud.WeApp.SDK.Tunnel;
using System;

namespace QCloud.WeApp.Tests
{
    public partial class TunnelServiceTest
    {
        [TestMethod]
        [Description("获取 WebSocket 连接地址")]
        public void TestGetConnectionWithSession()
        {
            var httpContextMock = helper.CreateTunnelHttpContextMock("GET", sessionType: "valid");
            var tunnelHandlerMock = new Mock<ITunnelHandler>();

            TunnelService tunnelService = new TunnelService(httpContextMock.Object.Request, httpContextMock.Object.Response);
            tunnelService.Handle(tunnelHandlerMock.Object, new TunnelHandleOptions() { CheckLogin = true });

            /// 验证是否成功调用 OnTunnelRequest
            /// 正常获取的时候，信道不为空，并且有 Id 可以进行索引
            /// 因为 CheckLogin 配置为 true，所以应该可以获取到用户信息
            tunnelHandlerMock.Verify(
                expression: x => x.OnTunnelRequest(
                    It.Is<SDK.Tunnel.Tunnel>(tunnel => tunnel.Id != null),
                    It.Is<SDK.Authorization.UserInfo>(user => user != null)
                ),
                times: Times.Once(),
                failMessage: "没有正确调用 OnTunnelRequest"
            );

            /// 验证是否正确输出申请到的连接地址到客户端
            httpContextMock.Verify(
               expression: x => x.Response.Write(
                   It.Is<string>(content => helper.CheckBodyHasUrl(content))
               ),
               times: Times.Once(),
               failMessage: "请求 WebSocket 连接没有进行合法的输出"
            );
        }


        [TestMethod]
        [Description("无会话获取 WebSocket 连接地址")]
        public void TestGetConnectionWithoutSession()
        {
            var httpContextMock = helper.CreateTunnelHttpContextMock("GET", sessionType: null);
            var tunnelHandlerMock = new Mock<ITunnelHandler>();

            TunnelService tunnelService = new TunnelService(httpContextMock.Object.Request, httpContextMock.Object.Response);
            tunnelService.Handle(tunnelHandlerMock.Object, new TunnelHandleOptions() { CheckLogin = false });

            /// 验证是否成功调用 OnTunnelRequest
            /// 正常获取的时候，信道不为空，并且有 Id 可以进行索引
            /// 因为 CheckLogin 配置为 false，所以应该获取不到用户信息
            tunnelHandlerMock.Verify(
                expression: x => x.OnTunnelRequest(
                    It.Is<SDK.Tunnel.Tunnel>(tunnel => tunnel.Id != null),
                    It.Is<SDK.Authorization.UserInfo>(user => user == null)
                ),
                times: Times.Once(),
                failMessage: "没有正确调用 OnTunnelRequest"
            );

            /// 验证是否正确输出申请到的连接地址到客户端
            httpContextMock.Verify(
               expression: x => x.Response.Write(
                   It.Is<string>(content => helper.CheckBodyHasUrl(content))
               ),
               times: Times.Once(),
               failMessage: "请求 WebSocket 连接没有进行合法的输出"
            );
        }


        [TestMethod]
        [Description("无效会话获取 WebSocket 连接地址")]
        public void TestGetConnectionWithInvalidSession()
        {
            var httpContextMock = helper.CreateTunnelHttpContextMock("GET", sessionType: "invalid");
            var tunnelHandlerMock = new Mock<ITunnelHandler>();

            TunnelService tunnelService = new TunnelService(httpContextMock.Object.Request, httpContextMock.Object.Response);
            tunnelService.Handle(tunnelHandlerMock.Object, new TunnelHandleOptions() { CheckLogin = true });

            /// 验证是否调用 OnTunnelRequest
            /// 请求登录的情况下，如果会话无效，应该直接输出会话无效
            tunnelHandlerMock.Verify(
                expression: x => x.OnTunnelRequest(
                    It.IsAny<SDK.Tunnel.Tunnel>(),
                    It.IsAny<SDK.Authorization.UserInfo>()
                ),
                times: Times.Never(),
                failMessage: "意外地调用了 OnTunnelRequest"
            );            

            /// 验证是否正确输出申请到的连接地址到客户端
            httpContextMock.Verify(
               expression: x => x.Response.Write(
                   It.Is<string>(content => helper.CheckBodyHasMagicId(JObject.Parse(content)))
               ),
               times: Times.Once(),
               failMessage: "请求 WebSocket 连接没有进行合法的输出"
            );
        }
    }
}
