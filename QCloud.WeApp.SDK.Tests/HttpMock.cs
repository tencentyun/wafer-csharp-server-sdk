using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QCloud.WeApp.SDK.Tests
{
    public class HttpMock
    {
        public Mock<HttpRequestBase> Request { get; set; }
        public Mock<HttpResponseBase> Response { get; set; }
    }
}
