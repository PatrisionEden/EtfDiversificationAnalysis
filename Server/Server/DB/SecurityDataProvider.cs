using System.Data;
using System.Data.SQLite;
using System.Linq;
using Newtonsoft.Json;

namespace Server
{
    public class SecurityDataProvider
    {
        private static string _dbFileName = "../EtfParser/db1.db";
        private DBConnection _dbConnection;


        private static SecurityDataProvider? _instanse;

        private SecurityDataProvider()
        {
            _dbConnection = new DBConnection(_dbFileName);
        }

        public static SecurityDataProvider GetInstance()
        {
            if (_instanse == null)
                _instanse = new();
            return _instanse;
        }

        public Dictionary<string, string> GetIsinToTickerDictionary()
        {
            Dictionary<string, string> isinToTicker = new Dictionary<string, string>();

            GetAllEtf().ForEach(e => isinToTicker[e.Isin] = e.Ticker);
            GetAllShares().ForEach(sh => isinToTicker[sh.Isin] = sh.Ticker);

            return isinToTicker;
        }

        public Dictionary<string, double> GetIsinToPriceDictionary()
        {
            Dictionary<string, double> isinToTicker = new Dictionary<string, double>();

            GetAllEtf().ForEach(e => isinToTicker[e.Isin] = e.Price);
            GetAllShares().ForEach(sh => isinToTicker[sh.Isin] = sh.Price);

            return isinToTicker;
        }

        public List<Security> GetAllEtf()
        {
            List<Security> etfs = new List<Security>();
            var tableRows = _dbConnection.GetTableFromSql(Queries.GetEtfsTable).Rows;
            foreach (DataRow row in tableRows)
            {
                string ticker = row[1].ToString() ?? "";
                string isin = row[5].ToString() ?? "";
                double price = (row[2].ToString() == null || row[2].ToString() == "") ? 0 : double.Parse(row[2]!.ToString());
                Security security = new EtfFound(isin, ticker, price);
                etfs.Add(security);
            }
            return etfs;
        }
        public List<Security> GetAllShares()
        {
            List<Security> shares = new List<Security>();
            var tableRows = _dbConnection.GetTableFromSql(Queries.GetSharesTable).Rows;
            foreach (DataRow row in tableRows)
            {
                string ticker = row[2].ToString() ?? "";
                string isin = row[0].ToString() ?? "";
                double price = (row[6].ToString() == null || row[6].ToString() == "") ? 0 : double.Parse(row[6].ToString());
                Security security = new EtfFound(isin, ticker, price);
                shares.Add(security);
            }
            return shares;
        }

        public EtfFound GetEtfByIsin(string isin)
        {
            var response = _dbConnection.GetTableFromSql(Queries.GetEtfByIsin(isin)).Rows[0];
            string isinFromTable = response[5].ToString() ?? "";
            string ticker = response[1].ToString() ?? "";
            string priceInString = response[2].ToString() ?? "";
            double price = (priceInString == "") ? 0 : double.Parse(priceInString);
            EtfFound result = new EtfFound(isin, ticker, price);
            return result;
        }

        public List<KeyValuePair<string, double>> GetEtfCompositionByIsin(string isin)
        {
            var listJson = _dbConnection.GetTableFromSql(Queries.GetShareIsinToPartListJsonFromEtfsTableByIsin(isin))
                .Rows[0][0].ToString();
            Dictionary<string, double>? compositionDictionary = JsonConvert.DeserializeObject<Dictionary<string, double>>(listJson ?? "[]");

            List<KeyValuePair<string, double>> composition = compositionDictionary!.ToList<KeyValuePair<string, double>>();

            return composition;
        }
        public Share GetShareByIsin(string isin)
        {
            var response = _dbConnection.GetTableFromSql(Queries.GetShareByIsin(isin)).Rows[0];
            string isinFromTable = response["Isin"].ToString() ?? "";
            string name = response["ShareName"].ToString() ?? "";
            string ticker = response["Ticker"].ToString() ?? "";
            string priceInString = response["Price"].ToString() ?? "";
            double price = (priceInString == "") ? 0 : double.Parse(priceInString);

            string country = response["Country"].ToString() ?? "";
            string sector = response["Sector"].ToString() ?? "";
            string industry = response["Industry"].ToString() ?? "";

            Share result = new Share(isinFromTable, ticker, name, price, country, sector, industry);
            return result;
        }

        public double GetPriceByIsin(string isin)
        {
            if (this.IsThisEtf(isin))
                return this.GetEtfByIsin(isin).Price;
            else
                return this.GetShareByIsin(isin).Price;
        }

        public bool IsThisEtf(string isin)
        {
            var dbResponse = _dbConnection
                .GetTableFromSql(Queries.CountOfEtfWithSpecifiedIsin(isin))
                .Rows[0]
                .ToString();
            return dbResponse != "0";
        }
    }
}
