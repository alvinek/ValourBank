using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;


namespace ValourBankApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var srvConf = new ServerConfiguration();
            var srv = new SimpleServer(srvConf.GetUri());
            srv.ServerListenerStart();
        }
    }

}