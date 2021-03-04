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
        public static int AccountState;

        static async Task Main(string[] args)
        {
            //ValourBankApi.SImpleObsfucator.PtrCreate();
            UICMD.UICMD.InitializeConsoleProps();
            while (true)
            {

                //Hashowanie md5
                password = Crypto.HashIt.Encrypt(password);
                //Connect Attempt
                try
                {
                    await Task.Run(() => ValourBankApi.EventHandler.RequestAsync(login, password));
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
            await Task.Run(() => UICMD.UICMD.InitializedMenu());

            Console.ReadKey();
        }




    }

}

