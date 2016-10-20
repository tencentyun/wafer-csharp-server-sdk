using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QCloud.WeApp.SDK;
using QCloud.WeApp.SDK.Authorization;
using QCloud.WeApp.TestServer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace QCloud.WeApp.Tests
{
    [TestClass]
    public partial class LoginServiceTest
    {

        private LoginServiceTestHelper helper = new LoginServiceTestHelper();


        [TestMethod]
        [Description("构造函数中 request 或 response 为 null 抛出异常")]
        public void TestConstructorWithNullRequest()
        {
            List<ArgumentNullException> exceptions = new List<ArgumentNullException>();
            var mock = helper.CreateLoginHttpMock(null, null);
            try
            {
                new LoginService(null, mock.Object.Response);
            }
            catch (ArgumentNullException ex)
            {
                exceptions.Add(ex);
            }
            try
            {
                new LoginService(mock.Object.Request, null);
            }
            catch (ArgumentNullException ex)
            {
                exceptions.Add(ex);
            }
            Assert.AreEqual(exceptions.Count, 2);
        }

    }
}
