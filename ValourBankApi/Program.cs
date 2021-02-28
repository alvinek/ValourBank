using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValourBankApi
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerConfiguration serverConfiguration = new ServerConfiguration();
            SimpleServer simpleServer = new SimpleServer(serverConfiguration.serverUri);
            simpleServer.ServerListenerStart();
        }
    }
}
