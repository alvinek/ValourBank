using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ValourBankApi
{
    class SImpleObsfucator
    {

        unsafe public static void PtrCreate(uint notptr=32)
        {
            uint* ptr = (uint*)notptr;
            Console.WriteLine((uint)&notptr);
        }
    }
}