using MyPassword.Infra;
using MyPassword.Infra.Data;
using System.ComponentModel;

namespace MyPassword
{
    public class EditAccountPageViewModel : INotifyPropertyChanged
    {
        private AccountRepository _accountRepository;
        private string _id;
        private string _login;
        private string _password;
        private string _description;
        private string _btnText;
        private bool _isEdit;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                RaisePropertyChanged("Id");
            }
        }

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                RaisePropertyChanged("Login");
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChanged("Password");
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                RaisePropertyChanged("Description");
            }
        }

        public string BtnText
        {
            get => _btnText;
            set
            {
                _btnText = value;
                RaisePropertyChanged("BtnText");
            }
        }

        public Command SaveBtn { get; }

        public EditAccountPageViewModel(string id)
        {
            SaveBtn = new Command(SaveAccountData);
            _accountRepository = AccountRepository.GetInstance();
            if (id == null)
            {
                PrepareForCreate();
            }
            else
            {
                var account = _accountRepository.GetById(id);
                PrepareForEdit(account);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        private void SaveAccountData(object obj)
        {
            if (_isEdit)
            {
                var res = _accountRepository.UpdateAccount(new Account
                {
                    Id = Id,
                    Login = Login,
                    Password = Password,
                    Description = Description
                });
            }
            else
            {
                var res = _accountRepository.CreateAccount(new Account
                {
                    Login = Login,
                    Password = Password,
                    Description = Description
                });
                Id = res.Id;
                BtnText = "Зберігти";
            }
        }

        private void PrepareForEdit(Account account)
        {
            _isEdit = true;
            BtnText = "Зберігти";
            if(account == null)
            {
                PrepareForCreate();
            }
            else
            {
                Id = account.Id;
                Login = account.Login;
                Password = account.Password;
                Description = account.Description;
            }
        }

        private void PrepareForCreate()
        {
            _isEdit = false;
            BtnText = "Додати";
        }
    }
}
