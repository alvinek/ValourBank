using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.ReadKey();
            await RequestAsync();
            Console.WriteLine("DONE");
        }

        public static async Task RequestAsync()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/");
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