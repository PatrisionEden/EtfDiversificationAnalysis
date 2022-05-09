namespace Server
{
    public class InitialData
    {
        public UserData UserData;
        public List<KeyValuePair<string, double>> IsinToPrice;
        public List<KeyValuePair<string, string>> IsinToTicker;
        //public List<KeyValuePair<string, string>> AvailableEtfs;
        public InitialData(UserData userData, List<KeyValuePair<string, double>> isinToPrice,
            List<KeyValuePair<string, string>> isinToTicker)
        {
            UserData = userData;
            IsinToPrice = isinToPrice;
            IsinToTicker = isinToTicker;
            //AvailableEtfs = availableEtfs;
        }
        public static InitialData Create(UserData userData, List<Security> etfs)
        {
            List<KeyValuePair<string, double>> _isinToPrice = new List<KeyValuePair<string, double>>();
            List<KeyValuePair<string, string>> _isinToTicker = new List<KeyValuePair<string, string>>();
            //List<KeyValuePair<string, string>> _availableEtfs = new List<KeyValuePair<string, string>>();

            foreach(var etf in etfs)
            {
                _isinToPrice.Add(new KeyValuePair<string, double>(etf.Isin, etf.Price));
                _isinToTicker.Add(new KeyValuePair<string, string>(etf.Isin, etf.Ticker));
                //_availableEtfs.Add(new KeyValuePair<string, string>(etf.Isin, etf.));
            }
            return new InitialData(userData, _isinToPrice, _isinToTicker);
        }
    }
}
