using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QCloud.WeApp.Tests
{
    class LoginServiceTestHelper
    {
        public Mock<HttpContextBase> CreateLoginHttpMock(string code, string encryptData)
        {
            var mock = new Mock<HttpContextBase>();
            mock.Setup(x => x.Request.HttpMethod).Returns("POST");
            mock.Setup(x => x.Request.Headers).Returns(() =>
            {
                var headers = new NameValueCollection();
                if (code != null) headers.Add("X-WX-Code", code);
                if (encryptData != null) headers.Add("X-WX-Encrypt-Data", encryptData);
                return headers;
            });
            mock.Setup(x => x.Response.Write(null));
            return mock;
        }

        public Mock<HttpContextBase> CreateCheckHttpMock(string id, string skey)
        {
            var mock = new Mock<HttpContextBase>();
            mock.Setup(x => x.Request.HttpMethod).Returns("POST");
            mock.Setup(x => x.Request.Headers).Returns(() =>
            {
                var headers = new NameValueCollection();
                if (id != null) headers.Add("X-WX-Id", id);
                if (skey != null) headers.Add("X-WX-Skey", skey);
                return headers;
            });
            mock.Setup(x => x.Response.Write(null));
            return mock;
        }

        public bool CheckBodyHasMagicId(JObject body)
        {
            return body["F2C224D4-2BCE-4C64-AF9F-A6D872000D1A"].Value<int>() == 1;
        }

        public bool CheckBodyHasSession(JObject body)
        {
            var session = body["session"];
            if (session?["id"] == null || session?["skey"] == null)
            {
                return false;
            }
            return true;
        }
    }
}
