using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ValourBankApi
{
    internal class ServerConfiguration
    {
        internal string urlPrefix = "http://localhost:{0}/";
        internal Uri serverUri;

        internal ServerConfiguration(int portNumber = 8080)
        {
            urlPrefix = string.Format(urlPrefix, portNumber);
            if (Uri.TryCreate(urlPrefix, UriKind.RelativeOrAbsolute, out serverUri))
            {
                Console.WriteLine("Server config created");
            }
            else
            {
                Console.WriteLine("Server config creation failed");
                throw new Exception("Could not create uri");
            }
        }

        internal Uri GetUri() => serverUri;

    }
}
