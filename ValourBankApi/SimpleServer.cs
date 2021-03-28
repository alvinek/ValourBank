using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ValourBankApi
{
    internal class SimpleServer
    {
        public HttpListener Listener;
        public Uri Uri;
        public SimpleDb simpleDb;

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
                        var user = request.QueryString["login"];
                        var passw = request.QueryString["pass"];

                        strResponse = simpleDb.LoginCheck(user, passw);

                        Debug.WriteLine($"{request.Url} => {strResponse}");
                    }
                }

                if (request.HttpMethod == "GET" && request.Url.AbsolutePath.StartsWith("/accdata"))
                {
                    if (request.QueryString.HasKeys())
                    {
                        var guid = request.QueryString["guid"];
                        strResponse = simpleDb.GetAccountState(guid);
                        Debug.WriteLine($"{request.Url} => {strResponse}");
                    }
                }

                if (request.HttpMethod == "GET" && request.Url.AbsolutePath.StartsWith("/accstate"))
                {
                    if (request.QueryString.HasKeys())
                    {
                        var guid = request.QueryString["guid"];
                        var state = request.QueryString["state"];

                        decimal.TryParse(state, NumberStyles.Float, CultureInfo.InvariantCulture, out var result);

                        strResponse = simpleDb.UpdateAccountState(guid, result);
                        Debug.WriteLine($"{request.Url} => {strResponse}");
                    }
                }

                if (request.HttpMethod == "GET" && request.Url.AbsolutePath.StartsWith("/logout"))
                {
                    if (request.QueryString.HasKeys())
                    {
                        var guid = request.QueryString["guid"];

                        simpleDb.DestroySession(guid);

                        Debug.WriteLine($"{guid} => Session destroyed");
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
            Prepare();
            Console.WriteLine("Server started on " + Uri);
            var timer = new Timer((x) => simpleDb.Flush(), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            var task = HandleConnection();
            task.GetAwaiter().GetResult();
            Listener.Close();
        }

        private void Prepare()
        {
            Listener = new HttpListener();
            Listener.Prefixes.Add(Uri.ToString());
            Listener.Start();
            simpleDb = new SimpleDb();
        }
    }
}
