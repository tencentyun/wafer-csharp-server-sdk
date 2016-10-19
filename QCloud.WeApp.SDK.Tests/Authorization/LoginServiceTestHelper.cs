using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QCloud.WeApp.SDK.Tests.Authorization
{
    class LoginServiceTestHelper
    {
        public HttpMock CreateLoginHttpMock(string code, string encryptData)
        {
            var requestMock = new Mock<HttpRequestBase>();
            var responseMock = new Mock<HttpResponseBase>();
            requestMock.Setup(x => x.HttpMethod).Returns("POST");
            requestMock.Setup(x => x.Headers).Returns(() =>
            {
                var headers = new NameValueCollection();
                if (code != null) headers.Add("X-WX-Code", code);
                if (encryptData != null) headers.Add("X-WX-Encrypt-Data", encryptData);
                return headers;
            });
            return new HttpMock()
            {
                Request = requestMock,
                Response = responseMock
            };
        }

        public HttpMock CreateCheckHttpMock(string id, string skey)
        {
            var requestMock = new Mock<HttpRequestBase>();
            var responseMock = new Mock<HttpResponseBase>();
            requestMock.Setup(x => x.HttpMethod).Returns("POST");
            requestMock.Setup(x => x.Headers).Returns(() =>
            {
                var headers = new NameValueCollection();
                if (id != null) headers.Add("X-WX-Id", id);
                if (skey != null) headers.Add("X-WX-Skey", skey);
                return headers;
            });
            return new HttpMock()
            {
                Request = requestMock,
                Response = responseMock
            };
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
