using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace QCloud.WeApp.SDK.Tests.Tunnel
{
    [TestClass]
    public partial class TunnelServiceTest
    {

        private static TestServer.TestServer server;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            ConfigurationManager.SetupFromFile(Directory.GetCurrentDirectory() + "\\sdk.test.config");
            server = new TestServer.TestServer(System.Configuration.ConfigurationManager.AppSettings["test-server-url"]);
            server.Start();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            server.Stop();
        }

        private TunnelServiceTestHelper helper = new TunnelServiceTestHelper();
        
    }
}
