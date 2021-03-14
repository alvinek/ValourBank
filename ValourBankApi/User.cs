using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValourBankApi
{
    class User
    {
        internal string Username { get; set; } = string.Empty;
        internal string Password { get; set; } = string.Empty;
        internal decimal AccountState { get; set; } = 0;
        internal string Guid { get; set; } = string.Empty;
    }
}
