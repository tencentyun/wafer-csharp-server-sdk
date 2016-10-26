using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QCloud.WeApp.SDK.Authorization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;

namespace QCloud.WeApp.Tests
{
    public partial class LoginServiceTest
    {
        [TestMethod]
        [Description("正常登陆流程，应该返回用户信息并且输出会话")]
        public void TestLoginProcess()
        {
            var mock = helper.CreateLoginHttpMock(code: "valid-code", encryptData: "valid-data");

            var loginService = new LoginService(mock.Object.Request, mock.Object.Response);
            UserInfo userInfo = loginService.Login();

            Assert.IsNotNull(userInfo);

            Func<string, bool> bodyMatch = (string body) =>
            {
                JObject result = JObject.Parse(body);
                return helper.CheckBodyHasMagicId(result) && helper.CheckBodyHasSession(result);
            };

            mock.Verify(x => x.Response.Write(It.Is((string body) => bodyMatch(body))), Times.Once());
        }

        [TestMethod]
        [Description("登陆头不包含 code 或 encryptData，应该抛出异常并且输出错误")]
        public void TestLoginProcessWithoutCodeOrData()
        {
            var errors = new LoginServiceException[] {
                TestLoginProcessExpectError(null, "valid-data"),
                TestLoginProcessExpectError("valid-code", null),
            }.Where(x => x != null);
            Assert.AreEqual(errors.Count(), 2);
        }

        [TestMethod]
        [Description("登陆头 code 或 encryptData 不正确，应该抛出异常并且输出错误")]
        public void TestLoginProcessWithInvalidCodeOrData()
        {
            var errors = new LoginServiceException[] {
                TestLoginProcessExpectError("invalid-code", "valid-data"),
                TestLoginProcessExpectError("valid-code", "invalid-data")
            }.Where(x => x != null);
            Assert.AreEqual(errors.Count(), 2);
        }

        [TestMethod]
        [Description("服务器鉴权服务器响应格式不正确，应该抛出异常并且输出错误")]
        public void TestLoginProcessWithServerResponseError()
        {
            var errors = new LoginServiceException[] {
                TestLoginProcessExpectError("expect-invalid-json", "valid-data"),
            }.Where(x => x != null);
            Assert.AreEqual(errors.Count(), 1);
        }

        [TestMethod]
        [Description("服务器鉴权服务器 500，应该抛出异常并且输出错误")]
        public void TestLoginProcessWithServer500()
        {
            var errors = new LoginServiceException[] {
                TestLoginProcessExpectError("expect-500", "valid-data"),
            }.Where(x => x != null);
            Assert.AreEqual(errors.Count(), 1);
        }

        [TestMethod]
        [Description("服务器鉴权服务器响应超时，应该抛出异常并且输出错误")]
        public void TestLoginProcessWithServerTimeout()
        {
            var errors = new LoginServiceException[] {
                TestLoginProcessExpectError("expect-timeout", "valid-data"),
            }.Where(x => x != null);
            Assert.AreEqual(errors.Count(), 1);
        }

        private LoginServiceException TestLoginProcessExpectError(string code, string encryptData)
        {
            var mock = helper.CreateLoginHttpMock(code, encryptData);

            var loginService = new LoginService(mock.Object.Request, mock.Object.Response);

            LoginServiceException errorShouldThrow = null;
            try
            {
                UserInfo userInfo = loginService.Login();
            }
            catch (LoginServiceException error)
            {
                errorShouldThrow = error;
            }

            Func<string, bool> bodyMatch = (string body) =>
            {
                JObject result = JObject.Parse(body);
                return helper.CheckBodyHasMagicId(result) && result["error"] != null;
            };

            mock.Verify(x => x.Response.Write(It.Is((string body) => bodyMatch(body))), Times.Once());

            return errorShouldThrow;
        }
    }
}
