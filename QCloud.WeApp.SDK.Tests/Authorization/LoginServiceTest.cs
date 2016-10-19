using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QCloud.WeApp.SDK.Authorization;
using System.Web;
using System.Linq;
using Moq;
using Moq.Language.Flow;
using System.Collections.Specialized;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

namespace QCloud.WeApp.SDK.Tests.Authorization
{
    [TestClass]
    public partial class LoginServiceTest
    {
        private static TestServer.TestServer server;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            ConfigurationManager.SetupFromFile(Directory.GetCurrentDirectory() + "\\sdk.test.config");
            server = new TestServer.TestServer(System.Configuration.ConfigurationManager.AppSettings["test-server-url"]);
            server.Start();
            Thread.Sleep(100);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            server.Stop();
        }

        private LoginServiceTestHelper helper = new LoginServiceTestHelper();


        [TestMethod]
        [Description("构造函数中 request 或 response 为 null 抛出异常")]
        public void TestConstructorWithNullRequest()
        {
            List<ArgumentNullException> exceptions = new List<ArgumentNullException>();
            HttpMock mock = helper.CreateLoginHttpMock(null, null);
            try
            {
                new LoginService(null, mock.Response.Object);
            }
            catch (ArgumentNullException ex)
            {
                exceptions.Add(ex);
            }
            try
            {
                new LoginService(mock.Request.Object, null);
            }
            catch (ArgumentNullException ex)
            {
                exceptions.Add(ex);
            }
            Assert.AreEqual(exceptions.Count, 2);
        }

    }
}
