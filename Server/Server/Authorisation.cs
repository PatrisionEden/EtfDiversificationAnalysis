namespace Server
{
    public class Authorisation
    {
        private static Authorisation? _instance;

        private List<string> _registeredUsers;
        private Dictionary<string, string> _loginToPassword;
        private Dictionary<string, string> _loginToToken;
        private Dictionary<string, DateTime> _tokenToExpirationDateTime;

        private Dictionary<string, UserData> _userDatas;

    private Authorisation()
        {
            _registeredUsers = new List<string>();
            _loginToPassword = new Dictionary<string, string>();
            _loginToToken = new Dictionary<string, string>();
            _tokenToExpirationDateTime = new Dictionary<string, DateTime>();
            _userDatas = new Dictionary<string, UserData>();

            _registeredUsers.Add("user");
            _loginToPassword["user"] = "user";
            _userDatas["user"] = new UserData("user", "Administrator", new() { 
                    new Security("0001", 1),
                    new Security("0002", 2),
                    new Security("0003", 3),
                    new Security("0004", 4)
            });
        }

        public static Authorisation GetInstace()
        {
            if (_instance == null)
                _instance = new Authorisation();
            return _instance;
        }

        public string RegisterNewUser(UserRegData userRegData)
        {
            if (_registeredUsers.Contains(userRegData.login))
                throw new AuthorizationException("Такой уже есть");

            _registeredUsers.Add(userRegData.login);
            _loginToPassword[userRegData.login] = userRegData.password;
            _userDatas[userRegData.login] = new UserData(userRegData.login, userRegData.login, new List<Security>());

            return AuthorizeUserAndGetTocken(new UserLoginData(userRegData.login, userRegData.password));
        }

        public string AuthorizeUserAndGetTocken(UserLoginData userLoginData)
        {
            if (!_registeredUsers.Contains(userLoginData.login))
                throw new AuthorizationException("Такого пользователя нет");
            if (_loginToPassword[userLoginData.login] != userLoginData.password)
                throw new AuthorizationException("Пароль не подошел");

            string tocken = GenerateNewTocken();
            _loginToToken[userLoginData.login] = tocken;
            return tocken;
        }

        public string GenerateNewTocken()
        {
            Random rnd = new Random();
            string tocken = "";
            for (int i = 0; i < 10; i++)
                tocken += rnd.NextInt64(0, 9);
            if (_loginToToken.ContainsValue(tocken))
                return GenerateNewTocken();

            _tokenToExpirationDateTime[tocken] = DateTime.Now.AddMinutes(50);
            return tocken;
        }

        public bool IsThisTockenForThatLoginValid(string login, string tocken)
        {
            if (!_loginToToken.ContainsKey(login) ||
                _loginToToken[login] != tocken)
                return false;
            if (!_tokenToExpirationDateTime.ContainsKey(tocken))
                throw new AuthorizationException("Токена нет в дикшинари со сроком действия, хотя должен быть");
            if (_tokenToExpirationDateTime[tocken] < DateTime.Now)
            {
                _loginToToken.Remove(login);
                _tokenToExpirationDateTime.Remove(tocken);

                return false;
            }

            return true;
        }

        public UserData GetUserDataByLogin(string login)
        {
            return _userDatas[login];
        }
        public void SetUserDataByLogin(string login, UserData userData)
        {
            _userDatas[login] = userData;
        }
        public void SaveProtfolio(string login, List<Security> securities)
        {
            _userDatas[login].Portfolio = securities;
        }
    }
    public class AuthorizationException : Exception
    {
        public AuthorizationException() : base()
        {
            
        }
        public AuthorizationException(string message) : base(message)
        {

        }
    }
}
