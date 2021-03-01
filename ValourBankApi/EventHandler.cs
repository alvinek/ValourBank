using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
namespace ValourBankApi
{
    class EventHandler
    {
        public static bool IsAccountExists(string confirmation)
        {
            if (confirmation=="true")
                return true;
            else
                return false;
        }
        public static async Task RequestAsync(string login, string password)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/?login=" + login + "&passwordhash=" + password); request.ContentType = "text/html";
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    Program.recieved_data = reader.ReadToEnd();
                }
            }
            response.Close();
        }

        public static async Task RequestAsync()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/?AccData");
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    Program.recieved_data = reader.ReadToEnd();
                }
            }
            response.Close();
        }
    }
}
