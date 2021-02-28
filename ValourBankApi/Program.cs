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
        static async Task Main(string[] args)
        {
            string login = "ACURASA", password = "123";
            password = Crypto.HashIt.Encrypt(password);
            Console.WriteLine(password);
            await RequestAsync(login, password);
        }



        public static async Task RequestAsync(string login,string password)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("http://localhost:8080/?login={login},passwordhash={password}"));
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    Console.WriteLine(reader.ReadToEnd());
                }
            }
            response.Close();
        }
    }
    
}

