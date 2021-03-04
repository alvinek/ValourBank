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
            Program.login = Console.ReadLine();
            Console.WriteLine("Now, Enter your password");
            Program.password = Console.ReadLine();
        }
        public static async Task InitializedMenu()
        {
            while (true)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.Clear();
                Console.WriteLine("Successful login, welcome back");
                await Task.Run(() => ValourBankApi.EventHandler.GetAccountState()); Thread.Sleep(1000);
            Badref:
                Console.WriteLine("Your Current deposit is... " + Program.AccountState);
                Console.WriteLine("1:\t Withdraw ");
                Console.WriteLine("2:\t Deposit ");
                Console.WriteLine("Q: Quit Application ");
                string option = Console.ReadLine(); int result;
                if (option == "Q" || option == "q")
                {
                    //await ValourBankApi.EventHandler.CloseConnectionAsync();
                    break;
                }
                if (Int32.TryParse(option, out result))
                {
                    int localResult;
                    if (result == 1 || result == 2)
                    {
                        Console.WriteLine("How much you want to withdraw | deposit? ...\t"); option = Console.ReadLine();
                        if (Int32.TryParse(option, out localResult))
                        {
                            if (localResult <= Program.AccountState && localResult >= 0 && result == 1)
                            {
                                Program.AccountState -= localResult;
                                await ValourBankApi.EventHandler.SetAccountState();
                            }
                            else if (localResult >= 0 && result == 2)
                            {
                                Program.AccountState += localResult;
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
