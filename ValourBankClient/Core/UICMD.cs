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
        public static void InitializeConsoleProps()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Red;
            Printlogo(); Thread.Sleep(500); Printlogo(); Thread.Sleep(500);
            Console.WriteLine("Switala & Stasyuk Bank"); Thread.Sleep(500);
            Console.WriteLine("Please, Log in... \n Enter your client ID");
            ValourBankApi.Includes.dlc.login = Console.ReadLine();
            Console.WriteLine("Now, Enter your password");
            ValourBankApi.Includes.dlc.password = Console.ReadLine();
        }
        public static async Task InitializedMenu()
        {
            while (true)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.Clear();
                Console.WriteLine("Successful login, welcome back " + ValourBankApi.Includes.dlc.login);
                await Task.Run(() => ValourBankApi.EventHandler.GetAccountState()); Thread.Sleep(1000);
            Badref:
                Console.WriteLine("Your Current deposit is... " + ValourBankApi.Includes.dlc.accountState);
                Console.WriteLine("1:\t Withdraw ");
                Console.WriteLine("2:\t Deposit ");
                Console.WriteLine("Q: Quit Application ");
                string option = Console.ReadLine(); int result;
                if (option == "Q" || option == "q")
                {
                    await ValourBankApi.EventHandler.LogoutAccount();
                    break;
                }
                if (Int32.TryParse(option, out result))
                {
                    Double localResult;
                    if (result == 1 || result == 2)
                    {
                        Console.WriteLine("How much you want to withdraw | deposit? ...\t"); option = Console.ReadLine();
                        Console.WriteLine("Max Withdraw is 100 000");
                        Console.WriteLine("Max deposit is 100 000");
                        if (Double.TryParse(option, out localResult))
                        {
                            if (localResult <= ValourBankApi.Includes.dlc.accountState && localResult >= 0 && result == 1)
                            {
                                ValourBankApi.Includes.dlc.accountState -= localResult;
                                await ValourBankApi.EventHandler.SetAccountState();
                            }
                            else if (localResult >= 0 && result == 2 && localResult <= 100000)
                            {
                                ValourBankApi.Includes.dlc.accountState += localResult;
                                await ValourBankApi.EventHandler.SetAccountState();
                            }
                            else
                            {
                                Console.WriteLine("Invalid Operation, Try Again\t"); goto Badref;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Operation, Try Again\t"); goto Badref;
                        }
                    }

                }
                else
                {
                    Console.WriteLine("Invalid Operation, Try Again\t"); goto Badref;
                }
            }
        }
        private static void Printlogo()
        {
            Console.WriteLine("W *****************\n" +
                              "E *****************\n" +
                              "L ***          ****\n" +
                              "C ***          ****\n" +
                              "O ***              \n" +
                              "M ****************♥\n" +
                              "E *****************\n" +
                              "T               ***\n" +
                              "O ****          ***\n" +
                              "S ****          ***\n" +
                              "S *****************\n" +
                              "B *****************\n");
        }

    }
}
