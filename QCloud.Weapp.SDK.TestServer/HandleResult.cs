using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK.TestServer
{
    public class HandleResult
    {
        public DateTime Time { get; set; }
        public string Url { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public int Status { get; internal set; }
        public string Method { get; internal set; }

        public override string ToString()
        {
            var spliter = "\r\n========================\r\n";
            var log = $"{Method.ToUpper()} {Url}";
            log += spliter;
            log += Request == null ? "(empty)" : Request;
            log += "\r\n\r\n";
            log += $"Response {Status}";
            log += spliter;
            log += Response == null ? "(empty)" : Response;
            log += "\r\n\r\n";

            return log;
        }
    }
}
