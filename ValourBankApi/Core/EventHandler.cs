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
        public static string separator = "&";
        public static bool IsAccountExists(string confirmation)
        {
            if (confirmation == "true")
                return true;
            else
                return false;
        }
        public static async Task RequestAsync(string login, string password)
        {
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/?login=" + login + "&passwordhash=" + password); 
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/?" + login + separator + password);
            request.ContentType = "text/html"; request.UserAgent = "SSB";
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    ValourBankApi.Includes.dlc.recieved_data = reader.ReadToEnd();
                }
            }
            response.Close();
        }

        public static async Task GetAccountState()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/?AccData");
            request.ContentType = "text/html"; request.UserAgent = "SSB";
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    ValourBankApi.Includes.dlc.recieved_data = reader.ReadToEnd();
                }
                Double.TryParse(ValourBankApi.Includes.dlc.recieved_data, out ValourBankApi.Includes.dlc.accountState);
            }
            response.Close();
        }
        public static async Task SetAccountState()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/?Update");
            request.ContentType = "text/html"; request.UserAgent = "SSB";
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    ValourBankApi.Includes.dlc.recieved_data = reader.ReadToEnd();
                    
                }
            }
            response.Close();
        }
        public static async Task CloseConnectionAsync()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/shutdown");
            request.ContentType = "text/html"; request.UserAgent = "SSB";
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    ValourBankApi.Includes.dlc.recieved_data = reader.ReadToEnd();
                }
            }
            response.Close();
        }
    }
}
