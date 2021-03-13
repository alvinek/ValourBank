using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValourBankApi
{
    static class Methods
    {
        private static int count = 0;

        internal static string Shutdown()
        {
            return "Shutdown requested";
        }

        internal static int Counter()
        {
            return count++;
        }
    }
}
