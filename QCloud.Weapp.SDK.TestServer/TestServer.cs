using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK.TestServer
{
    public class TestServer
    {
        public delegate void OnHandleResultCallback(HandleResult result);
        public event OnHandleResultCallback OnServerHandleResult;

        private HttpListener http;
        private Thread thread;

        public TestServer(string serverUrl)
        {
            http = new HttpListener();
            http.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            http.Prefixes.Clear();
            http.Prefixes.Add(serverUrl);
        }

        public void Start()
        {
            http.Start();
            thread = new Thread(new ThreadStart(delegate
            {
                Console.WriteLine($"Test server listening at {http.Prefixes.First()}");
                while (http.IsListening)
                {
                    HttpListenerContext context;
                    try
                    {
                        context = http.GetContext();
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine(error);
                        return;
                    }
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;
                    HttpHandler handler = new HttpHandler(request, response);
                    HandleResult result = handler.Handle();
                    if (OnServerHandleResult != null)
                    {
                        OnServerHandleResult.Invoke(result);
                    }
                }
            }));
            thread.Start();
        }

        public void Stop()
        {
            if (thread != null)
            {
                http.Stop();
                thread.Interrupt();
                thread = null;
            }
        }
    }
}
