using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.TestServer
{
    public static class HttpListenerExtend
    {
        public static string GetRawEntityBody(this HttpListenerRequest request)
        {
            if (request.HasEntityBody)
            {
                using (StreamReader reader = new StreamReader(request.InputStream))
                {
                    return reader.ReadToEnd();
                }
            }
            return null;
        }

        public static void Write(this HttpListenerResponse response, string content)
        {
            using (StreamWriter writer = new StreamWriter(response.OutputStream))
            {
                writer.Write(content);
            }
        }
    }
}
