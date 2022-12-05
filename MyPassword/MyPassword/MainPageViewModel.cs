using MyPassword.Infra;
using MyPassword.Infra.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MyPassword
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private AccountRepository _accountRepository;
        private ObservableCollection<Account> _accounts;

        public ObservableCollection<Account> Accounts 
        { 
            get => _accounts; 
            private set
            {
                _accounts = value;
                RaisePropertyChanged("Accounts");
            }
        }
        public MainPageViewModel()
        {
            SearchBtn = new Command(Search);
            DeleteAccountBtn = new Command(DeleteAccount);
            EditAccountBtn = new Command(EditAccount);
            CopyPassBtn = new Command(CopyPassword);
            CreateBtn = new Command(CreateAccount);
            ExportBtn = new Command(ExportAccounts);
            _accountRepository = AccountRepository.GetInstance();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Command SearchBtn { get; }
        public Command DeleteAccountBtn { get; }
        public Command EditAccountBtn { get; }
        public Command CopyPassBtn { get; }
        public Command CreateBtn { get; }
        public Command ExportBtn { get; }

        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                RaisePropertyChanged("SearchText");
            }
        }

        private Account _selectedAccount;
        public Account SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                if (_selectedAccount != value)
                {
                    _selectedAccount = value;
                    App.Current.MainPage.DisplayAlert("Інформація", value.Password, "OK");
                }
            }
        }


        private void Search(object obj)
        {
            Accounts = new ObservableCollection<Account>(_accountRepository.SearchAccounts(SearchText));
        }

        private async void DeleteAccount(object obj)
        {
            var account = obj as Account;

            var res = await App.Current.MainPage.DisplayAlert("Інформація", $"Ви точно хочете видалити {account.Description}?", "OK", "Відміна"); ;
            if (res)
            {
                _accountRepository.DeleteAccount(account.Id);
                Accounts.Remove(account);
            }
        }

        private async void EditAccount(object obj)
        {
            var account = obj as Account;

            await App.Current.MainPage.Navigation.PushAsync(new EditAccountPage(account.Id));
        }

        private async void CreateAccount(object obj)
        {
            await App.Current.MainPage.Navigation.PushAsync(new EditAccountPage(null));
        }

        private async void ExportAccounts()
        {
            await App.Current.MainPage.DisplayAlert("Інформаційний банер", "Експорт скоро почнется", "OK");

            await Task.Delay(2000);

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Експорт акаунтів",
                File = new ShareFile(AccountRepository.GetInstance().GetFullPath())
            });
        }

        private async void CopyPassword(object obj)
        {
            await Clipboard.Default.SetTextAsync(obj.ToString());
        }

        private void RaisePropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        public void LoadData()
        {
            Accounts = new ObservableCollection<Account>(_accountRepository.GetAllAccounts());
        }
    }
}
