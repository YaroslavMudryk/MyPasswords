namespace MyPassword.Infra
{
    public class Snapshot
    {
        public DateTime LastSync { get; set; }
        public ClientInfo Info { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
    }
}