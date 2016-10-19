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

namespace QCloud.WeApp.SDK.Tests.Authorization
{
    public partial class LoginServiceTest
    {
        [TestMethod]
        [Description("检查登陆正常流程")]
        public void TestCheck()
        {
            HttpMock mock = helper.CreateCheckHttpMock("valid-id", "valid-key");
            LoginService service = new LoginService(mock.Request.Object, mock.Response.Object);
            Console.WriteLine("Check");
            UserInfo userInfo = service.Check();
            Assert.IsNotNull(userInfo);

            mock.Response.Verify(x => x.Write(It.IsAny<string>()), Times.Never());
        }

        [TestMethod]
        [Description("请求头不包含 Id 或 Skey，抛出异常并响应错误，但类型不是会话无效")]
        public void TestCheckWithoutIdOrSkey()
        {
            var errors = new LoginServiceException[] {
                TestCheckExpectError(null, "valid-key"),
                TestCheckExpectError("valid-id", null)
            }.Where(x => x != null);
            Assert.AreEqual(errors.Count(), 2);
        }


        [TestMethod]
        [Description("请求头包含不合法 Id 或 Skey，抛出异常并响应错误，但类型不是会话无效")]
        public void TestCheckWithInvalidIdOrSkey()
        {
            var errors = new LoginServiceException[] {
                TestCheckExpectError("invalid-id", "valid-key", false),
                TestCheckExpectError("valid-id", "invalid-key", false)
            }.Where(x => x != null);
            Assert.AreEqual(errors.Count(), 2);
        }
        
        [TestMethod]
        [Description("服务器响应 60011 或 60012，抛出异常，类型是会话无效")]
        public void TestCheckWithInvalidSession()
        {
            var errors = new LoginServiceException[] {
                TestCheckExpectError("expect-60011", "valid-key", true),
                TestCheckExpectError("expect-60012", "valid-key", true)
            }.Where(x => x != null);
            Assert.AreEqual(errors.Count(), 2);
        }

        [TestMethod]
        [Description("服务器响应非法 JSON，抛出异常，类型不是会话无效")]
        public void TestCheckWithServerInvalidResponse()
        {
            var errors = new LoginServiceException[] {
                TestCheckExpectError("expect-invalid-json", "valid-key", false)
            }.Where(x => x != null);
            Assert.AreEqual(errors.Count(), 1);
        }

        
        [TestMethod]
        [Description("服务器 500，抛出异常，类型不是会话无效")]
        public void TestCheckWithServer500()
        {
            var errors = new LoginServiceException[] {
                TestCheckExpectError("expect-500", "valid-key", false)
            }.Where(x => x != null);
            Assert.AreEqual(errors.Count(), 1);
        }
        

        [TestMethod]
        [Description("服务器超时，抛出异常，类型不是会话无效")]
        public void TestCheckWithServerTimeout()
        {
            var errors = new LoginServiceException[] {
                TestCheckExpectError("expect-timeout", "valid-key", false)
            }.Where(x => x != null);
            Assert.AreEqual(errors.Count(), 1);
        }

        private LoginServiceException TestCheckExpectError(string id, string skey, bool? expectInvalidSession = null)
        {
            var mock = helper.CreateCheckHttpMock(id, skey);

            var loginService = new LoginService(mock.Request.Object, mock.Response.Object);

            LoginServiceException errorShouldThrow = null;
            try
            {
                UserInfo userInfo = loginService.Check();
            }
            catch (LoginServiceException error)
            {
                errorShouldThrow = error;
            }

            Func<string, bool> bodyMatch = (string body) =>
            {
                Console.WriteLine("=======Response========");
                Console.WriteLine(body);
                JObject result = JObject.Parse(body);
                if (!helper.CheckBodyHasMagicId(result))
                {
                    return false;
                }
                if (expectInvalidSession == null) return true;
                return result["error"].Value<string>() == "ERR_INVALID_SESSION" ^ !expectInvalidSession.Value;
            };

            mock.Response.Verify(x => x.Write(It.Is((string body) => bodyMatch(body))), Times.Once());

            return errorShouldThrow;
        }
    }
}
