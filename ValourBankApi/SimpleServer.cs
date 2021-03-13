using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
                var strResponse = string.Empty;

                Console.WriteLine("Request: " + request.Url);
                Console.WriteLine("Method: " + request.HttpMethod);
                Console.WriteLine("UserHostName: " + request.UserHostName);
                Console.WriteLine("UserAgent: " + request.UserAgent);

                if (request.HttpMethod == "GET" && request.Url.AbsolutePath.StartsWith("/shutdown"))
                {
                    strResponse = Methods.Shutdown();
                    serverRunning = false;
                }

                if (request.HttpMethod == "GET" && request.Url.AbsolutePath.StartsWith("/login"))
                {
                    
                    if (request.QueryString.HasKeys())
                    {
                        var passw = request.ContentEncoding.GetBytes(request.QueryString["passw"]);
                        var user = request.ContentEncoding.GetBytes(request.QueryString["user"]);
                        var passwUtf8 = Encoding.Convert(request.ContentEncoding, Encoding.UTF8, passw);
                        var userUtf8 = Encoding.Convert(request.ContentEncoding, Encoding.UTF8, user);
                         

                        strResponse =
                            $"{Encoding.UTF8.GetString(passwUtf8)} {Encoding.UTF8.GetString(userUtf8)}";

                    }
                }

                var data = Encoding.UTF8.GetBytes(strResponse);
                context.Response.ContentType = "text/html";
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.ContentLength64 = data.LongLength;

                await context.Response.OutputStream.WriteAsync(data, 0, data.Length);
                context.Response.Close();
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
