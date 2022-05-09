namespace Server
{
    public class UserData
    {
        public string Login;
        public string UserName;
        public List<SecurityData> Portfolio;

        public UserData(string login, string userName, List<SecurityData> portfolio)
        {
            Login = login;
            UserName = userName;
            Portfolio = portfolio;
        }
    }
    public class SecurityData
    {
        public readonly string Isin;
        public readonly int Amount;

        public SecurityData(string isin, int amount)
        {
            Isin = isin;
            Amount = amount;
        }
    }
}
