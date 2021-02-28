using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ValourBankApi
{
    internal class SimpleServer
    {
        public HttpListener Listener;
        public Uri Uri;

        internal SimpleServer(Uri uri)
        {
            Uri = uri;
        }

        internal async Task HandleConnection()
        {
            bool serverRunning = true;

            while (serverRunning)
            {
                var context = await Listener.GetContextAsync();

                var request = context.Request;
                var response = context.Response;

                Console.WriteLine("Request: " + request.Url);
                Console.WriteLine("Method: " + request.HttpMethod);
                Console.WriteLine("UserHostName: " + request.UserHostName);
                Console.WriteLine("UserAgent: " + request.UserAgent);

                if (request.HttpMethod == "GET" && request.Url.AbsolutePath.StartsWith("/shutdown"))
                {
                    Methods.Shutdown();
                    serverRunning = false;
                }

                var data = Encoding.UTF8.GetBytes("Hello world.");
                response.ContentType = "text/html";
                response.ContentEncoding = Encoding.UTF8;
                response.ContentLength64 = data.LongLength;

                await response.OutputStream.WriteAsync(data, 0, data.Length);
                response.Close();
            }
        }


        internal void ServerListenerStart()
        {
            Listener = new HttpListener();
            Listener.Prefixes.Add(Uri.ToString());
            Listener.Start();

            Console.WriteLine("Server started on " + Uri);
            var task = HandleConnection();
            task.GetAwaiter().GetResult();
            Listener.Close();
        }
    }
}
