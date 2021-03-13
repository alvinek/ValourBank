using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValourBankApi
{
    class User
    {
        internal string Username { get; set; }
        internal string Password { get; set; }
        internal decimal AccountState { get; set; }
        internal string Guid { get; set; }
    }
}
