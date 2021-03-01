using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using ValourBankApi;
namespace UICMD
{
    static class UICMD
    {
        public static async Task InitializeConsoleProps()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Red;
            Printlogo(); Thread.Sleep(500); Printlogo(); Thread.Sleep(500);
            Console.WriteLine("Switala & Stasyuk Bank"); Thread.Sleep(500);
            Console.WriteLine("Please, Log in... \n Enter your client ID");
            Program.login = Console.ReadLine();
            Console.WriteLine("Now, Enter your password");
            Program.password = Console.ReadLine();
        }
        public static async Task InitializedMenu()
        {
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.Clear();
            Console.WriteLine("Successful login, welcome back");
        }
        private static void Printlogo()
        {
            Console.WriteLine("W *****************\n" +
                              "E *****************\n" +
                              "L ***           ***\n" +
                              "C ***              \n" +
                              "O ***              \n" +
                              "M *****************\n" +
                              "E *****************\n" +
                              "T               ***\n" +
                              "O               ***\n" +
                              "S ***           ***\n" +
                              "S *****************\n" +
                              "B *****************\n");
        }
    }
}
