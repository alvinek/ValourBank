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
            var test = new List<string> {"test1", "test2", "test3"};
            foreach(var r in test)
                Console.WriteLine(r);
            test.Single(x => x.Contains("test"));
            ServerConfiguration serverConfiguration = new ServerConfiguration();
            SimpleServer simpleServer = new SimpleServer(serverConfiguration.serverUri);
            simpleServer.ServerListenerStart();
        }
    }
}
