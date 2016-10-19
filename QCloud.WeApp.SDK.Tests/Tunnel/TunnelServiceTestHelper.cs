using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QCloud.WeApp.SDK.Tests.Tunnel
{
    class TunnelServiceTestHelper
    {
        public HttpMock CreateTunnelHttpMock(string method, bool includeSession)
        {
            var requestMock = new Mock<HttpRequestBase>();
            var responseMock = new Mock<HttpResponseBase>();
            requestMock.Setup(x => x.Url).Returns(new Uri("http://test.qcloud.la/tunnel"));
            requestMock.Setup(x => x.HttpMethod).Returns(method.ToUpper());
            requestMock.Setup(x => x.Headers).Returns(() =>
            {
                var headers = new NameValueCollection();
                if (includeSession)
                {
                    headers.Add("X-WX-Id", "valid-id");
                    headers.Add("X-WX-Skey", "valid-key");
                }
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
    }
}
