using System;

using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Crypto;
namespace ValourBankApi
{

    class Program
    {
        static async Task Main(string[] args)
        {
            UICMD.UICMD.InitializeConsoleProps();
            while (true)
            {

                //Hashowanie md5
                ValourBankApi.Includes.dlc.hashedPass = Crypto.HashIt.Encrypt(ValourBankApi.Includes.dlc.password);
                //Connect Attempt
                try
                {
                    await Task.Run(() => ValourBankApi.EventHandler.RequestAsync(ValourBankApi.Includes.dlc.login, ValourBankApi.Includes.dlc.password));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                //Verifying Account
                if (ValourBankApi.EventHandler.IsAccountExists(ValourBankApi.Includes.dlc.recieved_data))
                {
                    
                    break;
                }

                Console.WriteLine("\n Access denied, try again...");
                UICMD.UICMD.InitializeConsoleProps();
            }
            await Task.Run(() => UICMD.UICMD.InitializedMenu());
        }
    }

}

