using System;

using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Crypto;
namespace ValourBankApi
{

    class Program
    {
        public static string recieved_data;
        public static string login;
        public static string password;
        private static uint op_Id = 0;
        public static string Account
        {
            set 
            {
                op_Id++;
                Console.WriteLine("AccountState has been changed, operation ID is - " + op_Id  ); 
            }
            get
            { return Account; }

        }
        static async Task Main(string[] args)
        {
            while (true)
            {
                UICMD.UICMD.InitializeConsoleProps();
                //Hashowanie md5
                password = Crypto.HashIt.Encrypt(password);
                //Connect Attempt
                try
                {
                    await ValourBankApi.EventHandler.RequestAsync(login, password);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                //Verifying Account
                if (ValourBankApi.EventHandler.IsAccountExists(recieved_data)) 
                    break;
                Console.WriteLine("\n Access denied, try again...");
                UICMD.UICMD.InitializeConsoleProps();
            }
            UICMD.UICMD.InitializedMenu();
            Console.ReadKey();
        }



       
    }
    
}

