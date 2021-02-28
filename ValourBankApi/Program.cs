using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Crypto;
namespace ConsoleApp1
{

    class Program
    {
        private static string recieved_data { set; get; }
        static async Task Main(string[] args)
        {
            string login = "ACURASA", password = "123";
            password = Crypto.HashIt.Encrypt(password);
            Console.WriteLine(password);
            try
            {
                await RequestAsync(login, password);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadLine();
        }



        public static async Task RequestAsync(string login,string password)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/?login=" + login + ",passwordhash=" + password); request.ContentType = "text/html";
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                  Program.recieved_data  = reader.ReadToEnd();
                }
            }

            response.Close();
        }
    }
    
}

