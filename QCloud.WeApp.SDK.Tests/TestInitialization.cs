using Microsoft.VisualStudio.TestTools.UnitTesting;
using QCloud.WeApp.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QCloud.WeApp.Tests
{
    [TestClass]
    public static class TestInitialization
    {
        private static TestServer.TestServer server;

        [AssemblyInitialize]
        public static void Start(TestContext context)
        {
            ConfigurationManager.SetupFromFile(Directory.GetCurrentDirectory() + "\\sdk.test.config");
            server = new WeApp.TestServer.TestServer(System.Configuration.ConfigurationManager.AppSettings["test-server-url"]);
            server.Start();
        }

        [AssemblyCleanup]
        public static void Stop()
        {
            server.Stop();
        }
    }
}
