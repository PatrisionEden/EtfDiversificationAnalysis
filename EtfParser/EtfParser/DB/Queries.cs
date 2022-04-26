using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EtfParser.DB
{
    public static class Queries
    {
        public static string CreateIfNotExistsEtfProvadersTable()
            => "CREATE TABLE IF NOT EXISTS EtfProviders ( " +
                "EtfProviderName TEXT    UNIQUE " +
                "                        NOT NULL, " +
                "EtfNamesListJson TEXT, " +
                "EtfDatasListJson TEXT, " +
                "IsInitialised BOOLEAN NOT NULL " +
                ");";
        public static string CountOfRowsWithEtfProviderNameInEtfProvidersTable(string etfProviderName)
            => "SELECT Count() " +
                "FROM EtfProviders " +
                string.Format("Where EtfProviderName = '{0}'; ", etfProviderName);
        public static string InsertEtfProviderData(IEtfProviderData etfProviderData)
        {
            string etfNamesListJson = JsonConvert.SerializeObject(etfProviderData.EtfNames);
            string etfDatasListJson = JsonConvert.SerializeObject(etfProviderData.EtfDatas?.Select(ed => ed.EtfName).ToList());

            return "INSERT INTO EtfProviders( " +
                "         EtfProviderName, " +
                "         EtfNamesListJson, " +
                "         EtfDatasListJson, " +
                "         IsInitialised " +
                "     ) " +
                "     VALUES( " +
                string.Format("         '{0}', ", etfProviderData.EtfProviderName) +
                string.Format("         '{0}', ", etfNamesListJson) +
                string.Format("         '{0}', ", etfDatasListJson) +
                string.Format("         {0} ", etfProviderData.IsInitialised ? "TRUE" : "FALSE") +
                "     );";
        }
        public static string UpdateEtfProviderData(IEtfProviderData etfProviderData)
        {
            string etfNamesListJson = JsonConvert.SerializeObject(etfProviderData.EtfNames);
            string etfDatasListJson = JsonConvert.SerializeObject(etfProviderData.EtfDatas?.Select(ed => ed.EtfName).ToList());

            return "UPDATE EtfProviders " +
                   string.Format("SET EtfNamesListJson = '{0}', ", etfNamesListJson) +
                   string.Format("    EtfDatasListJson = '{0}', ", etfDatasListJson) +
                   string.Format("    IsInitialised = {0} ", etfProviderData.IsInitialised ? "TRUE" : "FALSE") +
                   string.Format("WHERE EtfProviderName = '{0}'", etfProviderData.EtfProviderName);
        }

        public static string GetEtfProviderData(string etfProviderName)
            => "SELECT * " +
                "FROM EtfProviders " +
                string.Format("WHERE EtfProviderName = '{0}'; ", etfProviderName);

        public static string CreateIfNotExistsEtfsTable()
            => "CREATE TABLE IF NOT EXISTS Etfs( " +
                "EtfName TEXT    UNIQUE " +
                "                NOT NULL, " +
                "Ticker UNIQUE " +
                "       NOT NULL, " +
                "Cost DOUBLE, " +
                "ShareIsinListJson TEXT, " +
                "IsinToPartInEtfDictionrayJson TEXT, " +
                "IsInitialised BOOLEAN NOT NULL " +
                "                                      DEFAULT (FALSE) " +
                ");";

        public static string CountOfRowsWithEtfNameInEtfsTable(string etfName)
            => "SELECT Count() " +
                "FROM Etfs " +
                string.Format("Where EtfName = '{0}'; ", etfName);
        public static string InsertEtfDataInEtfsTable(IEtfData etfData)
        {
            string shareIsinListJson = JsonConvert.SerializeObject(etfData.ShareDatas?.Select(sd => sd.Isin).ToList());
            string isinToPartInEtfDictionrayJson = JsonConvert.SerializeObject(etfData.IsinToPartInEtf);
            return "INSERT INTO Etfs ( " +
                    "EtfName, " +
                    "Ticker, " +
                    "Cost, " +
                    "ShareIsinListJson, " +
                    "IsinToPartInEtfDictionrayJson, " +
                    "IsInitialised " +
                    ") " +
                    "VALUES( " +
                    string.Format("'{0}', ", etfData.EtfName) +
                    string.Format("'{0}', ", etfData.Ticker) +
                    string.Format("'{0}', ", etfData.Cost?.ToString().Replace(',', '.')) +
                    string.Format("'{0}', ", shareIsinListJson) +
                    string.Format("'{0}', ", isinToPartInEtfDictionrayJson) +
                    string.Format("'{0}' ", etfData.IsInitialised ? "TRUE" : "FALSE") +
                    ");";
        }
        public static string UpdateEtfDataInEtfsTable(IEtfData etfData)
        {
            string shareIsinListJson = JsonConvert.SerializeObject(etfData.ShareDatas?.Select(sd => sd.Isin).ToList());
            string isinToPartInEtfDictionrayJson = JsonConvert.SerializeObject(etfData.IsinToPartInEtf);
            return "UPDATE Etfs " +
                   string.Format("SET Ticker = '{0}', ", etfData.Ticker) +
                   string.Format("    Cost = '{0}', ", etfData.Cost?.ToString().Replace(',', '.')) +
                   string.Format("    ShareIsinListJson = '{0}', ", shareIsinListJson) +
                   string.Format("    IsinToPartInEtfDictionrayJson = '{0}', ", isinToPartInEtfDictionrayJson) +
                   string.Format("    IsInitialised = '{0}' ", etfData.IsInitialised ? "TRUE" : "FALSE") +
                   string.Format("WHERE EtfName = '{0}'; ", etfData.EtfName);
        }

        public static string GetEtfData(string etfName)
            => "SELECT * " +
                "FROM Etfs " +
                string.Format("WHERE EtfName = '{0}'; ", etfName);
        public static string CreateIfNotExistsSharesTable()
            => "CREATE TABLE IF NOT EXISTS Shares( " +
                "Isin TEXT     UNIQUE, " +
                "ShareName TEXT, " +
                "Ticker TEXT, " + 
                "Country TEXT, " +
                "Sector TEXT, " +
                "Industry TEXT, " +
                "Price DOUBLE, " +
                "IsInitialised BOOLEAN NOT NULL " +
                "); ";
        public static string CountOfRowsWithIsinInSharesTable(string isin)
            => "SELECT Count() " +
                "FROM Shares " +
                string.Format("Where Isin = '{0}'; ", isin);
        public static string InsertShareDataInSharesTable(IShareData shareData)
            =>  "INSERT INTO Shares ( " +
                "               Isin, " +
                "               ShareName, " +
                "               Ticker, " +
                "               Country, " +
                "               Sector, " +
                "               Industry, " +
                "               Price, " +
                "               IsInitialised " +
                "           ) " +
                "           VALUES( " +
                string.Format("'{0}', ", shareData.Isin) +
                string.Format("'{0}', ", shareData.ShareName.Replace("'", "''")) +
                string.Format("{0}, ", shareData.Ticker?.InQuotes() ?? "NULL") +
                string.Format("{0}, ", shareData.Country?.InQuotes() ?? "NULL") +
                string.Format("{0}, ", shareData.Sector?.InQuotes() ?? "NULL") +
                string.Format("{0}, ", shareData.Industry?.InQuotes() ?? "NULL") +
                string.Format("{0}, ", shareData.Price?.ToString().Replace(',', '.') ?? "NULL") +
                string.Format("'{0}' ", shareData.IsInitialised ? "TRUE" : "FALSE") +
                "           ); ";
        public static string UpdateShareDataInSharesTableByIsin(IShareData shareData)
            =>  "INSERT INTO Shares( " +
                "               Isin, " +
                "               ShareName, " +
                "               Ticker, " +
                "               Country, " +
                "               Sector, " +
                "               Industry, " +
                "               Price, " +
                "               IsInitialised " +
                "           ) " +
                "           VALUES( " +
                string.Format("'{0}', ", shareData.Isin) +
                string.Format("'{0}', ", shareData.ShareName.Replace("'", "''")) +
                string.Format("{0}, ",  shareData.Ticker?.InQuotes() ?? "NULL") +
                string.Format("{0}, ", shareData.Country?.InQuotes() ?? "NULL") +
                string.Format("{0}, ", shareData.Sector?.InQuotes() ?? "NULL") +
                string.Format("{0}, ", shareData.Industry?.InQuotes() ?? "NULL") +
                string.Format("{0}, ", shareData.Price?.ToString().Replace(',', '.') ?? "NULL") +
                string.Format("'{0}' ", shareData.IsInitialised ? "TRUE" : "FALSE") +
                "           );";
        public static string GetShareDataByIsin(string isin)
            => "SELECT * " +
            "FROM Shares " +
            string.Format("WHERE Isin = '{0}'", isin);
        private static string? InQuotes(this string str)
        {
            if (str == null)
                return null;
            return '\'' + str + '\'';
        }
    }
}

