using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;

namespace ValourBankTest
{
    [TestClass]
    public class UnitTest1
    {
        internal class TestUser
        {
            internal string Username { get; set; }
            internal string Password { get; set; }
            internal string Guid { get; set; }
            internal decimal AccountState { get; set; }
        }

        internal static class Urls
        {
            internal static string Login = "http://localhost:8080/login?login={0}&pass={1}";
            internal static string AccountData = "http://localhost:8080/accdata?guid={0}";
            internal static string AccountState = "http://localhost:8080/accstate?state={0}&guid={1}";
            internal static string Logout = "http://localhost:8080/logout?guid={0}";
            internal static string Transfer = "http://localhost:8080/transfer?guid={0}&dest={1}&much={2}";
        }

        private List<TestUser> testUsers = new List<TestUser>();

        [TestMethod, TestCategory("API")]
        public void SameUserTest()
        {
            var response = new WebClient().DownloadString(string.Format(Urls.Login, "dummy1", "dummypass1"));
            Assert.IsTrue(response.Contains(";"));
            var guidStr = response.Split(';')[1];
            Assert.IsTrue(Guid.TryParse(guidStr, out Guid result));
            response = new WebClient().DownloadString(string.Format(Urls.Login, "dummy1", "dummypass1"));
            Assert.AreEqual(response, "false");
            var logout = new WebClient().DownloadString(string.Format(Urls.Logout, guidStr));
            Assert.AreEqual(logout, string.Empty);
        }

        [TestMethod, TestCategory("API")]
        public void TransferMoneyTest()
        {
            var ids = Enumerable.Range(1, 499);

            Parallel.ForEach(ids, (id) =>
            {
                var response = new WebClient().DownloadString(string.Format(Urls.Login, $"dummy{id}", $"dummypass{id}"));
                Assert.IsTrue(response.Contains(";"));
                var guidStr = response.Split(';')[1];
                var amountOn1 = new WebClient().DownloadString(string.Format(Urls.AccountData, guidStr));
                Assert.IsTrue(decimal.TryParse(amountOn1, NumberStyles.Float, CultureInfo.InvariantCulture,
                    out var decimalAmountOn1));

                response = new WebClient().DownloadString(string.Format(Urls.Login, $"dummy{id+500}", $"dummypass{id+500}"));
                Assert.IsTrue(response.Contains(';'));
                var guidStr2 = response.Split(';')[1];
                var amountOn2 = new WebClient().DownloadString(string.Format(Urls.AccountData, guidStr2));

                Assert.IsTrue(decimal.TryParse(amountOn2, NumberStyles.Float, CultureInfo.InvariantCulture,
                    out var decimalAmountOn2));

                var transferMoney =
                    new WebClient().DownloadString(
                        string.Format(Urls.Transfer, guidStr, $"dummy{id+500}", "100"));

                var afterAmountOn2 = new WebClient().DownloadString(string.Format(Urls.AccountData, guidStr2));
                Assert.IsTrue(decimal.TryParse(afterAmountOn2, NumberStyles.Float, CultureInfo.InvariantCulture,
                    out var decimalAfterAmountOn2));

                if (decimalAmountOn1 - 100 < 0)
                {
                    Assert.AreEqual("false", transferMoney);
                    Assert.IsFalse(decimalAfterAmountOn2 - 100 == decimalAmountOn2);
                }
                else
                {
                    Assert.AreEqual("true", transferMoney);
                    Assert.IsTrue(decimalAfterAmountOn2 - 100 == decimalAmountOn2);
                }
               

                Assert.AreEqual(string.Empty, new WebClient().DownloadString(string.Format(Urls.Logout, guidStr2)));
                Assert.AreEqual(string.Empty, new WebClient().DownloadString(string.Format(Urls.Logout, guidStr)));
            }
            );
        }

        [TestMethod, TestCategory("API")]
        public void RunHeavyApiTest()
        {
            GenerateUsers();
            Assert.IsTrue(testUsers.Any());

            var concurrentDict = new ConcurrentDictionary<string, TestUser>();

            Parallel.ForEach(testUsers, new ParallelOptions{MaxDegreeOfParallelism = 4}, (user) =>
            {
                var response = new WebClient().DownloadString(string.Format(Urls.Login, user.Username, user.Password));
                var guid = string.Empty;
                if (response.Contains(";"))
                {
                    guid = response.Split(';')[1];
                }
                else
                {
                    Assert.Fail("Invalid response");
                }

                Assert.IsFalse(string.IsNullOrWhiteSpace(guid));
                Assert.IsTrue(Guid.TryParse(guid, out var result));

                var copyUser = user;
                copyUser.Guid = guid;

                var accountState = new WebClient().DownloadString(string.Format(Urls.AccountData, guid));

                Assert.IsTrue(decimal.TryParse(accountState, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal accData));
                copyUser.AccountState = accData;

                if(!concurrentDict.TryAdd(user.Username, copyUser)) Assert.Fail("Cannot add user");
            });

            testUsers = concurrentDict.Select(x => x.Value).ToList();
            concurrentDict.Clear();
            Assert.IsFalse(testUsers.All(x=>string.IsNullOrWhiteSpace(x.Guid)));
            int beforeCountRequests = 0, 
                afterCountRequests = 0;

            new Timer((x) =>
            {
                afterCountRequests = beforeCountRequests;
                beforeCountRequests = 0;
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            for (int i = 1; i <= 1000; i++)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Running phase {i} from 1000");
                int whichUser = 0;
                Parallel.ForEach(testUsers, new ParallelOptions(){MaxDegreeOfParallelism = 8}, (user) =>
                {
                    beforeCountRequests++;
                    Thread.Sleep(1);
                    var drawoutCash = new Random().Next(1000);
                    decimal expectedResult;

                    if (new Random().Next(0, 1) == 0)
                    {
                        if (user.AccountState > drawoutCash)
                        {
                            expectedResult = user.AccountState - drawoutCash;
                        }
                        else
                        {
                            expectedResult = user.AccountState + drawoutCash;
                        }
                    }
                    else
                    {
                        expectedResult = user.AccountState + drawoutCash;
                    }

                    var parsedString = expectedResult.ToString(CultureInfo.InvariantCulture);

                    new WebClient().DownloadString(string.Format(Urls.AccountState, parsedString, user.Guid));
                    var y = new WebClient().DownloadString(string.Format(Urls.AccountData, user.Guid));

                    Assert.IsTrue(decimal.TryParse(y, NumberStyles.Any, CultureInfo.InvariantCulture,
                        out var actualResult));
                    Assert.AreEqual(expectedResult, actualResult);

                    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {user.Username} = Success (had {user.AccountState} now is {actualResult}) (requests: {afterCountRequests}/sec)");
                    user.AccountState = actualResult;
                });
            }

            Parallel.ForEach(testUsers, (user) =>
            {
                var x = new WebClient().DownloadString(string.Format(Urls.Logout, user.Guid));
            });
        }

        public void GenerateUsers()
        {
            List<int> generatedIds = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                int userId;

                //generate users but be aware of duplicates
                do
                {
                    userId = new Random().Next(1, 999);
                } 
                while (generatedIds.Contains(userId));
                
                generatedIds.Add(userId);

                testUsers.Add(new TestUser()
                {
                    Username = "dummy" + userId,
                    Password = "dummypass" + userId
                });

                Thread.Sleep(1);
            }
        }


    }
}
