using System.Data;
using System.Data.SQLite;
using Newtonsoft.Json;

namespace Server
{
    public class UserDataController
    {
        private static UserDataController? _instance;
        private string _dbFileName = "usersDatas.db";
        private SQLiteConnection _dbConnection;

        private UserDataController()
        {
            _dbConnection =
                new SQLiteConnection(DBConnectionInitialisationStringTemplate(_dbFileName));
            _dbConnection.Open();
            CreateTablesIfNotExistTablesInitialization();
        }

        public static UserDataController GetInstance()
        {
            if (_instance == null)
                _instance = new UserDataController();
            return _instance;
        }

        public void RegisterNewUser(string userLogin, string userPassword)
        {
            if(IsUserAlreadyRegistered(userLogin))
                throw new AuthorizationException("Зареган уже блинб");
            string? userId = GetTableFromSql(Queries.AddNewUserInUsersTableAndGetItsId(userLogin)).Rows[0][0].ToString();
            if (userId == null)
                throw new NullReferenceException();
            ExecuteSqlCommand(Queries.AddPasswordToNewUserAndPortfolioRecord(userId, userPassword));
        }

        public bool IsUserAlreadyRegistered(string userLogin)
        {
            string? countInStr = GetTableFromSql(Queries.CountOfUsersByUserLogin(userLogin)).Rows[0][0].ToString(); 
            if (countInStr == null)
                throw new NullReferenceException();
            return countInStr != "0";
        }

        public void SavePortfolio(string userLogin, List<SecurityData> portfolio)
        {
            string? userId = GetTableFromSql(Queries.GetUserIdByUserLogin(userLogin)).Rows[0][0].ToString();
            if (userId == null)
                throw new NullReferenceException();
            string portfolioJson = JsonConvert.SerializeObject(portfolio);
            ExecuteSqlCommand(Queries.UpdateUserPortfolioJsonByUserId(userId, portfolioJson));
        }

        public bool CheckPassword(string userLogin, string userPassword)
        {
            string? userId = GetTableFromSql(Queries.GetUserIdByUserLogin(userLogin)).Rows[0][0].ToString();
            if (userId == null)
                throw new NullReferenceException();
            string? checkResult = GetTableFromSql(Queries.DoUserPasswordMatchById(userId, userPassword)).Rows[0][0].ToString();
            if (checkResult == null)
                throw new NullReferenceException();
            return checkResult == "1";
        }

        public List<SecurityData> GetUserPortfolioByUserLogin(string userLogin)
        {
            string? userId = GetTableFromSql(Queries.GetUserIdByUserLogin(userLogin)).Rows[0][0].ToString();
            if (userId == null)
                throw new NullReferenceException();
            string? userPortfolioJson = GetTableFromSql(Queries.GetUserPortfolioByUserId(userId)).Rows[0][0].ToString();
            if (userPortfolioJson == null)
                throw new NullReferenceException();
            List<SecurityData>? userPortfolioDeserialized = JsonConvert.DeserializeObject<List<SecurityData>>(userPortfolioJson);
            if (userPortfolioDeserialized == null)
                throw new NullReferenceException();
            return userPortfolioDeserialized;
        }

        public void SaveReport(string userLogin, string isin, string reportText)
        {
            string? userId = GetTableFromSql(Queries.GetUserIdByUserLogin(userLogin)).Rows[0][0].ToString();
            if (userId == null)
                throw new NullReferenceException();
            ExecuteSqlCommand(Queries.AddReport(userId, isin, reportText));
        }

        public List<Report> GetReports()
        {
            List<Report> reports = new List<Report>();
            var rows = GetTableFromSql(Queries.GetReports()).Rows;

            foreach(DataRow row in rows)
            {
                int reportId = int.Parse(row[0].ToString() ?? "-1");
                int userId = int.Parse(row[1].ToString() ?? "-1");
                string reportText = row[2].ToString() ?? "Error";
                string isin = row[3].ToString() ?? "Error";

                string userLogin = (string)GetTableFromSql(Queries.GetUserLoginById(userId)).Rows[0][0];

                reports.Add(new Report(reportId, userLogin, reportText, isin));
            }

            return reports;
        }

        private void CreateTablesIfNotExistTablesInitialization()
        {
            ExecuteSqlCommand(Queries.DBTablesInitialization());
        }

        private DataTable GetTableFromSql(string sql)
        {
            // Создаем объект DataAdapter
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, _dbConnection);
            // Создаем объект Dataset
            DataSet ds = new DataSet();
            // Заполняем Dataset
            adapter.Fill(ds);
            // Отображаем данные
            return ds.Tables[0];
        }

        private void ExecuteSqlCommand(string sql)
        {
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }

        private string DBConnectionInitialisationStringTemplate(string dbFileName)
            => string.Format("Data Source={0};", dbFileName);
    }
}
