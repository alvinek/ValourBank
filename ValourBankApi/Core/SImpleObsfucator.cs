using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ValourBankApi
{
    class SImpleObsfucator
    {

        unsafe public static void PtrCreate(uint notptr = 32)
        {
            uint* ptr = (uint*)notptr;
            Console.WriteLine((uint)&notptr);
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