﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Security.Permissions;

namespace ValourBankApi
{
    class EventHandler
    {
        public static string separator = "&";
        public static bool IsAccountExists(string confirmation)
        {
            if (confirmation.StartsWith("false"))
                return false;
            var split = confirmation.Split(';');
            Includes.dlc.guid = split[1];
            return true;
        }
        public static async Task RequestAsync(string login, string password)
        {
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/?login=" + login + "&passwordhash=" + password); 
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/login?login=" + login + separator + "pass=" + password);
            request.ContentType = "text/html"; request.UserAgent = "SSB";
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    ValourBankApi.Includes.dlc.recieved_data = await reader.ReadToEndAsync();
                }
            }
            response.Close();
        }

        public static async Task GetAccountState()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/accdata?guid=" + Includes.dlc.guid);
            request.ContentType = "text/html"; request.UserAgent = "SSB";
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    ValourBankApi.Includes.dlc.recieved_data = await reader.ReadToEndAsync();
                }
                Double.TryParse(ValourBankApi.Includes.dlc.recieved_data, out ValourBankApi.Includes.dlc.accountState);
            }
            response.Close();
        }
        public static async Task SetAccountState()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/accstate?state="
                                                                       +Includes.dlc.accountState+separator+"guid="+Includes.dlc.guid);
            request.ContentType = "text/html"; request.UserAgent = "SSB";
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    ValourBankApi.Includes.dlc.recieved_data = await reader.ReadToEndAsync();
                }
            }
            response.Close();
        }

        public static async Task TransferMoney(string toWho, double howMuch)
        {
            var urlRequested = $"http://localhost:8080/transfer?guid={Includes.dlc.guid}{separator}dest={toWho}{separator}much={howMuch}";

            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(urlRequested);

            req.ContentType = "text/html";
            req.UserAgent = "SSB";

            HttpWebResponse response = (HttpWebResponse) await req.GetResponseAsync();

            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    ValourBankApi.Includes.dlc.recieved_data = await reader.ReadToEndAsync();
                }
            }
        }

        public static async Task LogoutAccount()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/logout?guid=" + Includes.dlc.guid);
            request.ContentType = "text/html"; request.UserAgent = "SSB";
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    ValourBankApi.Includes.dlc.recieved_data = await reader.ReadToEndAsync();
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
                    ValourBankApi.Includes.dlc.recieved_data = await reader.ReadToEndAsync();
                }
            }
            response.Close();
        }
    }
}
