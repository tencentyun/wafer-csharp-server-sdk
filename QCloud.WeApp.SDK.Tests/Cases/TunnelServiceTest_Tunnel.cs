using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using Newtonsoft.Json.Linq;
using QCloud.WeApp.SDK;
using QCloud.WeApp.SDK.Authorization;
using QCloud.WeApp.SDK.Tunnel;
using System;
using Newtonsoft.Json;

namespace QCloud.WeApp.Tests
{
    public partial class TunnelServiceTest
    {

        [TestMethod]
        [Description("测试往信道发送消息")]
        public void TestTunnelEmit()
        {
            using (var mock = helper.CreateWebRequestProviderMock())
            {
                mock.SetupResponseBody(new { code = 0 }.ToJson());

                Tunnel tunnel = Tunnel.GetById("tunnel1");
                tunnel.Emit("test-type", "test-message");

                var packets = helper.ResolvePackets(mock.GetRequestContent());

                // emit 1 packet
                Assert.AreEqual(1, packets.Length);
                var firstPacket = packets[0];

                // the first packet should be a message packet
                Assert.AreEqual(1, firstPacket.tunnelIds.Length);
                Assert.AreEqual("tunnel1", firstPacket.tunnelIds[0]);
                Assert.AreEqual("message", firstPacket.type);

                // the message are encoded correctly
                var message = new { type = "", content = "" }.ParseFromJson(firstPacket.content);
                Assert.AreEqual("test-type", message.type);
                Assert.AreEqual("test-message", message.content);
            }
        }

        [TestMethod]
        [Description("测试往信道发送消息")]
        public void TestTunnelEmitWithInvalidTunnels()
        {
            using (var mock = helper.CreateWebRequestProviderMock())
            {
                mock.SetupResponseBody(new
                {
                    code = 0,
                    data = new
                    {
                        invalidTunnelIds = new string[] { "tunnel1" }
                    }.ToJson()
                }.ToJson());

                Tunnel tunnel = Tunnel.GetById("tunnel1");
                var result = tunnel.Emit("test-type", "test-message");
                Assert.IsTrue(result.HasTunnelInvalidInfo);
                Assert.AreEqual("tunnel1", result.TunnelIvalidInfos.First().TunnelId);
            }
        }

        [TestMethod]
        [Description("测试关闭信道")]
        public void TestTunnelClose()
        {
            using (var mock = helper.CreateWebRequestProviderMock())
            {
                mock.SetupResponseBody(new { code = 0 }.ToJson());

                Tunnel tunnel = Tunnel.GetById("tunnel1");
                tunnel.Close();

                var packets = helper.ResolvePackets(mock.GetRequestContent());

                // emit 1 packet
                Assert.AreEqual(1, packets.Length);

                // the first packet should be a close packet
                var firstPacket = packets[0];
                Assert.AreEqual(1, firstPacket.tunnelIds.Length);
                Assert.AreEqual("tunnel1", firstPacket.tunnelIds[0]);
                Assert.AreEqual("close", firstPacket.type);
            }
        }


        [TestMethod]
        [Description("测试房间与广播")]
        public void TestRoomBroadcast()
        {
            using (var mock = helper.CreateWebRequestProviderMock())
            {
                mock.SetupResponseBody(new { code = 0 }.ToJson());

                TunnelRoom room = new TunnelRoom();
                Assert.AreEqual(0, room.TunnelCount);

                room.AddTunnel(Tunnel.GetById("tunnel1"));
                room.AddTunnel(Tunnel.GetById("tunnel2"));
                Assert.AreEqual(2, room.TunnelCount);

                room.RemoveTunnelById("tunnel1");
                Assert.AreEqual(1, room.TunnelCount);

                room.AddTunnel(Tunnel.GetById("tunnel3"));
                room.Broadcast("test-type", "test-message");


                var packets = helper.ResolvePackets(mock.GetRequestContent());

                // emit 1 packet
                Assert.AreEqual(1, packets.Length);
                var firstPacket = packets[0];

                // the first packet should be a message packet
                Assert.AreEqual(2, firstPacket.tunnelIds.Length);
                Assert.AreEqual("tunnel2", firstPacket.tunnelIds[0]);
                Assert.AreEqual("tunnel3", firstPacket.tunnelIds[1]);
                Assert.AreEqual("message", firstPacket.type);

                // the message are encoded correctly
                var message = new { type = "", content = "" }.ParseFromJson(firstPacket.content);
                Assert.AreEqual("test-type", message.type);
                Assert.AreEqual("test-message", message.content);
            }
        }

        [TestMethod]
        [Description("测试房间与广播（包含无效信道）")]
        public void TestRoomBroadcastWithInvalidTunnels()
        {
            using (var mock = helper.CreateWebRequestProviderMock())
            {
                mock.SetupResponseBody(new
                {
                    code = 0,
                    data = new
                    {
                        invalidTunnels = new string[] { "tunnel1", "tunnel2" }
                    }.ToJson()
                }.ToJson());

                TunnelRoom room = new TunnelRoom();

                room.AddTunnel(Tunnel.GetById("tunnel1"));
                room.AddTunnel(Tunnel.GetById("tunnel2"));
                room.AddTunnel(Tunnel.GetById("tunnel3"));
                var result = room.Broadcast("test-type", "test-message");

                Assert.IsTrue(result.HasTunnelInvalidInfo);
                Assert.AreEqual(2, result.TunnelIvalidInfos.Count());
                Assert.AreEqual(2, result.TunnelIvalidInfos.Count(x => x.TunnelId == "tunnel1" || x.TunnelId == "tunnel2"));
                
            }
        }
    }
}
