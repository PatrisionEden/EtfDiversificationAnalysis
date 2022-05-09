namespace Server
{
    public static class Queries
    {
        public static string DBTablesInitialization()
            => "CREATE TABLE IF NOT EXISTS UsersTable (\n" +
                "    userId    INTEGER PRIMARY KEY AUTOINCREMENT\n" +
                "                      UNIQUE\n" +
                "                      NOT NULL,\n" +
                "    userLogin         UNIQUE\n" +
                "                      NOT NULL\n" +
                ");\n" +
                "\n" +
                "CREATE TABLE IF NOT EXISTS AuthorisationDataTable (\n" +
                "    userId       INTEGER REFERENCES UsersTable (userId) \n" +
                "                         NOT NULL\n" +
                "                         UNIQUE,\n" +
                "    userPassword STRING  NOT NULL\n" +
                ");\n" +
                "\n" +
                "CREATE TABLE IF NOT EXISTS UserPortfolioTable (\n" +
                "    userId            INTEGER REFERENCES UsersTable (userId) \n" +
                "                              NOT NULL\n" +
                "                              UNIQUE,\n" +
                "    userPortfolioJSON         NOT NULL\n" +
                "                              DEFAULT \"[]\"\n" +
                ");";

        public static string AddNewUserInUsersTableAndGetItsId(string userLogin)
            => "INSERT INTO UsersTable (\n" +
                "       userLogin\n" +
                "    )\n" +
                "    VALUES (\n" +
                string.Format("       '{0}'", userLogin) +
                "); \n" +
                "select MIN(userId) as id\n" +
                "from UsersTable\n" +
                string.Format("where userLogin = '{0}';", userLogin);
        public static string AddPasswordToNewUserAndPortfolioRecord(string userId, string userPassword)
            => "INSERT INTO AuthorisationDataTable (\n" +
                "       userId,\n" +
                "       userPassword\n" +
                "    )\n" +
                "    VALUES (\n" +
                string.Format("       '{0}',\n", userId) +
                string.Format("       '{0}'\n", userPassword) +
                ");\n" +
                "INSERT INTO UserPortfolioTable (\n" +
                "       userId\n" +
                "   )\n" +
                "   VALUES (\n" +
                string.Format("       '{0}'\n", userId) +
                ");";
        public static string UpdateUserPortfolioJsonByUserId(string userId, string userPortfolioJson)
            => "UPDATE UserPortfolioTable\n" +
                string.Format("SET userPortfolioJSON = '{0}'\n", userPortfolioJson) +
                string.Format("WHERE userId = '{0}';", userId);

        public static string GetUserIdByUserLogin(string userLogin)
            => "select MIN(userId) as id\n" +
                "from UsersTable\n" +
                string.Format("where userLogin = '{0}';", userLogin);
        public static string DoUserPasswordMatchById(string userId, string userPassword)
            => string.Format("select '{0}' = userPassword\n", userPassword) +
                "from AuthorisationDataTable\n" +
                string.Format("where userId = '{0}'", userId);
        public static string CountOfUsersByUserLogin(string userLogin)
            => "select count()\n" +
                "from UsersTable\n" +
                string.Format("where userLogin = '{0}'", userLogin);
        public static string GetUserPortfolioByUserId(string userId)
            => "select userPortfolioJSON\n" +
            "from UserPortfolioTable\n" +
            string.Format("where userId = {0};", userId);

        public static string GetEtfsTable
            => "select *\n" +
            "from Etfs";
        public static string GetEtfByIsin(string isin)
            => "select *\n" +
            "from Etfs\n" +
            string.Format("where Isin = '{0}';", isin);
        public static string CountOfEtfWithSpecifiedIsin(string isin)
            => "select Count()\n" +
            "from Etfs\n" +
            string.Format("where Isin = '{0}';", isin);
        public static string GetShareIsinToPartListJsonFromEtfsTableByIsin(string isin)
            => "select IsinToPartInEtfDictionrayJson\n" +
            "from Etfs\n" +
            string.Format("where Isin = '{0}';", isin);
        public static string GetShareByIsin(string isin)
            => "select *\n" +
            "from Shares\n" +
            string.Format("where Isin = '{0}'", isin);
    }
}
