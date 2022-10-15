using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyPassword.Infra.Data
{
    public class AccountRepository
    {
        private readonly string _filePath = Path.Combine(FileSystem.Current.AppDataDirectory, "accounts.json");
        private List<Account> _accounts;
        private static AccountRepository _accountRepository;
        private ClientInfo _clientInfo;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {

        };

        public static AccountRepository GetInstance()
        {
            if (_accountRepository == null)
                _accountRepository = new AccountRepository();
            return _accountRepository;
        }

        public AccountRepository()
        {
            LoadData();
        }

        public Account CreateAccount(Account account)
        {
            account.Id = GetNewId();
            _accounts.Add(account);

            SaveData();

            return account;
        }

        public Account UpdateAccount(Account account)
        {
            var accountToUpdate = _accounts.FirstOrDefault(s => s.Id == account.Id);

            accountToUpdate.Login = account.Login;
            accountToUpdate.Password = account.Password;
            accountToUpdate.Description = account.Description;

            var index = _accounts.IndexOf(accountToUpdate);
            _accounts[index] = accountToUpdate;

            SaveData();

            return accountToUpdate;
        }

        public void DeleteAccount(string id)
        {
            var accountToDelete = _accounts.FirstOrDefault(s => s.Id == id);
            if (accountToDelete != null)
            {
                _accounts.Remove(accountToDelete);
                SaveData();
            }
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            yield return new Account
            {
                Id = Guid.NewGuid().ToString("N").Substring(0, 10),
                Description = "тестова пошта",
                Login = "test@email.com",
                Password = "testPassword"
            };
        }

        public IEnumerable<Account> SearchAccounts(string query)
        {
            Func<Account, bool> predicate = (x) => x.Login.Contains(query) || x.Description.Contains(query);

            return _accounts.Where(predicate);
        }

        private void LoadData()
        {
            if (!File.Exists(_filePath))
            {
                var d = File.Create(_filePath);
                d.Close();
                _accounts = new List<Account>();
                _accounts.Add(new Account 
                { 
                    Id = Guid.NewGuid().ToString("N").Substring(0, 10),
                    Description = "тестова пошта",
                    Login = "test@email.com",
                    Password = "testPassword"
                });
                _clientInfo = GetInfo();
                return;
            }
            var jsonContent = File.ReadAllText(_filePath);

            var snapshot = JsonSerializer.Deserialize<Snapshot>(jsonContent, _jsonSerializerOptions);

            _clientInfo = snapshot.Info;
            _accounts = snapshot.Accounts.ToList();
        }

        private void SaveData()
        {
            var snapshot = new Snapshot
            {
                Info = GetInfo(),
                Accounts = _accounts,
                LastSync = DateTime.Now
            };

            var jsonContent = JsonSerializer.Serialize(snapshot, _jsonSerializerOptions);

            File.WriteAllText(_filePath, jsonContent);
        }

        private ClientInfo GetInfo()
        {
            var device = DeviceInfo.Current;
            return new ClientInfo
            {
                Brand = device.Manufacturer,
                Model = device.Model,
                OS = $"{device.Platform.ToString()} {device.VersionString}",
                DeviceId = Guid.NewGuid().ToString("N"),
            };
        }

        private string GetNewId()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 10);
        }
    }
}
