using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ValourBankApi
{
    class SimpleDb
    {
        private List<User> _users = new List<User>();
        private string dbPath = Path.Combine(Environment.CurrentDirectory, "db.bin");

        internal SimpleDb()
        {
            if (!File.Exists(dbPath))
            {
                DummyUsersCreator();
            }

            var linesEncrypted = File.ReadAllLines(dbPath);

            foreach (var lineEncrypted in linesEncrypted)
            {
                var decryptedLine = SimpleDbHash.Decrypt(lineEncrypted);

                var splittedLine = decryptedLine.Split(',');

                if (_users.Any(x => x.Username.Equals(splittedLine[0])))
                {
                    Console.WriteLine("Account already exists for this usr: " + splittedLine[0]);
                }

                if (decimal.TryParse(splittedLine[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var accountState))
                {
                    _users.Add(new User
                    {
                        AccountState = accountState,
                        Password = splittedLine[1],
                        Username = splittedLine[0]
                    });
                }
            }
        }

        internal string UpdateAccountState(string guid, decimal accountState)
        {
            if(_users.Any(x=>x.Guid.Equals(guid)))
            {
                _users.First(x=>x.Guid.Equals(guid)).AccountState = accountState;
                Flush();
                return "true";
            }

            return "false";
        }

        internal void DestroySession(string guid)
        {
            if (_users.Any(x => x.Guid.Equals(guid)))
            {
                _users.First(x => x.Guid.Equals(guid)).Guid = string.Empty;
            }
        }

        internal string GetAccountState(string guid)
        {
            if (_users.Any(x => x.Guid.Equals(guid)))
            {
                return _users.First(x => x.Guid.Equals(guid)).AccountState.ToString(CultureInfo.InvariantCulture);
            }
            return string.Empty;
        }

        internal string LoginCheck(string login, string password)
        {
            if (_users.Any(
                user => user.Username.Equals(login) && user.Password.Equals(password)
                ))
            {
                if (_users.Count(user => user.Username.Equals(login) && user.Password.Equals(password)) > 1)
                {
                    throw new Exception("More than one user has same login and password");
                }

                if (string.IsNullOrEmpty(_users.First(user =>
                    user.Username.Equals(login) && user.Password.Equals(password)).Guid))
                {
                    var guid = Guid.NewGuid();

                    _users.First(user => user.Username.Equals(login) && user.Password.Equals(password))
                        .Guid = guid.ToString();

                    return "true;" + guid;
                }
                // ktos jest juz zalogowany
                return "false";
                
            }
            else
            {
                return "false";
            }
        }

        internal void Flush()
        {
            List<string> db = _users
                .Select(user => 
                    SimpleDbHash.Encrypt($"{user.Username},{user.Password}," +
                                         $"{user.AccountState.ToString(CultureInfo.InvariantCulture)}"))
                .ToList();

            File.WriteAllLines(dbPath,db);
        }

        internal void DummyUsersCreator()
        {
            // robimy 1000 uzytkownikow

            for(int i = 0; i < 1000; i++)
            {
                var usr = new User()
                {
                    AccountState = new Random().Next(1, 99999),
                    Password = "dummy" + i,
                    Username = "dummypass" + i
                };
                _users.Add(usr);
                Thread.Sleep(5);
            }

            Flush();
        }
    }
}
