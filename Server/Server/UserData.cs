namespace Server
{
    public class UserData
    {
        public string Login;
        public string UserName;
        public List<Security> Portfolio;

        public UserData(string login, string userName, List<Security> portfolio)
        {
            Login = login;
            UserName = userName;
            Portfolio = portfolio;
        }
    }
    public class Security
    {
        public readonly string Isin;
        public readonly int Amount;

        public Security(string isin, int amount)
        {
            Isin = isin;
            Amount = amount;
        }
    }
}
