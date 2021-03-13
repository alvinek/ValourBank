using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
                var guid = Guid.NewGuid();

                _users.First(user => user.Username.Equals(login) && user.Password.Equals(password))
                    .Guid = guid.ToString();

                return "true;" + guid;
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
            var usr = new User()
            {
                AccountState = 50,
                Password = "strongpass",
                Username = "testuser"
            };

            _users.Add(usr);

            var usr2 = new User()
            {
                AccountState = (decimal) 50.5,
                Password = "2334445555",
                Username = "testuser2"
            };
            _users.Add(usr2);

            Flush();
        }
    }
}
