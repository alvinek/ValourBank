using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private bool changed = false;
        private bool Lock = false;
        private Timer timer = null;

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
                    Debug.WriteLine($"Reading user: {splittedLine[0]}");
                    _users.Add(new User
                    {
                        AccountState = accountState,
                        Password = splittedLine[1],
                        Username = splittedLine[0]
                    });
                }
            }
            
            timer = new Timer((x) => Flush(), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        internal string UpdateAccountState(string guid, decimal accountState)
        {
            WhileLock();
            if(_users.Any(x=>x.Guid.Equals(guid)))
            {
                _users.First(x=>x.Guid.Equals(guid)).AccountState = accountState;
                changed = true;
                return "true";
            }

            return "false";
        }

        internal string TransferMoney(string guid, string to, decimal howMuch)
        {
            WhileLock();

            if (_users.Any(x => x.Guid.Equals(guid)))
            {
                var userFrom = _users.First(x => x.Guid.Equals(guid));

                if (userFrom.AccountState > howMuch)
                {
                    var userTo = _users.FirstOrDefault(x => x.Username.Equals(to));
                    if (userTo != null)
                    {
                        userTo.AccountState += howMuch;
                        userFrom.AccountState -= howMuch;
                        changed = true;
                        return "true";
                    }
                }
            }

            return "false";

        }

        internal void DestroySession(string guid)
        {
            WhileLock();
            if (_users.Any(x => x.Guid.Equals(guid)))
            {
                _users.First(x => x.Guid.Equals(guid)).Guid = string.Empty;
            }
        }

        internal string GetAccountState(string guid)
        {
            WhileLock();
            if (_users.Any(x => x.Guid.Equals(guid)))
            {
                return _users.First(x => x.Guid.Equals(guid)).AccountState.ToString(CultureInfo.InvariantCulture);
            }
            return string.Empty;
        }

        internal string LoginCheck(string login, string password)
        {
            WhileLock();
            if (_users.Any(
                user => user.Username.Equals(login) && user.Password.Equals(password)
                ))
            {
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
            return "false";
        }

        internal void Flush()
        {
            if (!changed)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Flush has nothing to do");
                return;
            }
            if (Lock) return;

            changed = false;
            Lock = true;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Flush started");
            List<string> db = _users
                .Select(user =>
                {
                    Debug.WriteLine($"Flushing user: {user.Username}");
                    return SimpleDbHash.Encrypt($"{user.Username},{user.Password}," +
                                                $"{user.AccountState.ToString(CultureInfo.InvariantCulture)}");
                })
                .ToList();

            File.WriteAllLines(dbPath, db);
            Lock = false;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Flush finished");
        }

        internal void DummyUsersCreator()
        {
            // robimy 1000 uzytkownikow

            for(int i = 0; i < 1000; i++)
            {
                var usr = new User()
                {
                    AccountState = new Random().Next(1, 99999),
                    Username = $"dummy{i}",
                    Password = $"dummypass{i}"
                };
                _users.Add(usr);
                Thread.Sleep(5);

                if(i % 50 == 0)
                    Debug.WriteLine($"Creating users, {i} out of 1000");
            }

            changed = true;
        }

        internal void WhileLock()
        {
            while (Lock)
            {
                Thread.Sleep(10);
            }
        }
    }
}
