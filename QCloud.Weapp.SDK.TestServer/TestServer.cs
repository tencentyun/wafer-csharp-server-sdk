using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QCloud.WeApp.TestServer
{
    public class TestServer
    {
        public delegate void OnHandleResultCallback(HandleResult result);
        public event OnHandleResultCallback OnServerHandleResult;

        private HttpListener http;
        private Thread thread;

        public bool IsStarted
        {
            get
            {
                return http.IsListening;
            }
        }

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
                try
                {
                    while (http.IsListening)
                    {
                        HttpListenerContext context = http.GetContext();
                        // process in a new thread
                        new Thread(new ThreadStart(delegate
                        {
                            HttpListenerRequest request = context.Request;
                            HttpListenerResponse response = context.Response;
                            HttpHandler handler = new HttpHandler(request, response);
                            HandleResult result = handler.Handle();
                            if (OnServerHandleResult != null)
                            {
                                OnServerHandleResult.Invoke(result);
                            }
                        })).Start();
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }));
            thread.Start();
        }

        public void Stop()
        {
            if (http.IsListening)
            {
                http.Close();
            }
        }
    }
}
