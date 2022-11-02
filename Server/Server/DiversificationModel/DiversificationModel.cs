namespace Server
{
    public class DiversificationModel
    {
        public List<KeyValuePair<string, double?>> CountriesToPart;
        public List<KeyValuePair<string, double?>> SectorToPart;
        public List<KeyValuePair<string, double?>> IndustryToPart;
        //public List<Security> Securities;
        public List<KeyValuePair<string, double?>> IsinToPart;
        public KeyValuePair<string, string>[] IsinToTicker;
        public KeyValuePair<string, string>[] IsinToName;

        public DiversificationModel(List<KeyValuePair<string, double?>> countriesToPart, 
            List<KeyValuePair<string, double?>> sectorToPart, 
            List<KeyValuePair<string, double?>> industryToPart,
            List<KeyValuePair<string, double?>> isinToPart,
            KeyValuePair<string, string>[] isinToTicker,
            KeyValuePair<string, string>[] isinToName)
        {
            CountriesToPart = countriesToPart;
            SectorToPart = sectorToPart;
            IndustryToPart = industryToPart;
            IsinToPart = isinToPart;
            IsinToTicker = isinToTicker;
            IsinToName = isinToName;
        }

        public static DiversificationModel Create(string login)
        {
            List<KeyValuePair<string, double?>> countriesToPart = new List<KeyValuePair<string, double?>>();
            List<KeyValuePair<string, double?>> sectorToPart = new List<KeyValuePair<string, double?>>();
            List<KeyValuePair<string, double?>> industryToPart = new List<KeyValuePair<string, double?>>();
            //List<KeyValuePair<string, double?>> isinToPart = new List<Security>();
            List<KeyValuePair<string, double?>> isinToPart = new List<KeyValuePair<string, double?>>();
            Dictionary<string, string> isinToTicker = new Dictionary<string, string>();
            Dictionary<string, string> isinToName = new Dictionary<string, string>();

            SecurityDataProvider securityDataProvider = SecurityDataProvider.GetInstance();
            UserDataController userDataController = UserDataController.GetInstance();
            List<SecurityData> portfolio = userDataController.GetUserPortfolioByUserLogin(login);

            double portfolioPrice = portfolio
                .Select(sec => sec.Amount * securityDataProvider.GetPriceByIsin(sec.Isin))
                .Sum();

            foreach (var security in portfolio)
            {
                double priceOfSecurity = securityDataProvider
                    .GetPriceByIsin(security.Isin);

                double securityPartInPortfolio 
                    = security.Amount * priceOfSecurity / portfolioPrice;

                if (securityDataProvider.IsThisEtf(security.Isin))
                {
                    EtfFound etfFound = securityDataProvider.GetEtfByIsin(security.Isin);
                    List<KeyValuePair<string, double>> composition = securityDataProvider.GetEtfCompositionByIsin(security.Isin);
                    foreach(var compositionItem in composition)
                    {
                        Share share = securityDataProvider.GetShareByIsin(compositionItem.Key);
                        double partInEtf = compositionItem.Value;
                        double partInPortfolio = securityPartInPortfolio * partInEtf;
                        string ticker = share.Ticker;
                        string Name = share.Name;

                        if(partInPortfolio == 0)
                        {
                            ;
                        }

                        countriesToPart.AddingWithoutDublicates(new KeyValuePair<string, double?>(share.Country, partInPortfolio));
                        sectorToPart.AddingWithoutDublicates(new KeyValuePair<string, double?>(share.Sector, partInPortfolio));
                        industryToPart.AddingWithoutDublicates(new KeyValuePair<string, double?>(share.Industry, partInPortfolio));
                        isinToPart.AddingWithoutDublicates(new KeyValuePair<string, double?>(share.Isin, partInPortfolio));
                        isinToTicker[share.Isin] = share.Ticker;
                        isinToName[share.Isin] = share.Name;
                        var a = isinToName.ToArray();
                    }
                }
                else
                {
                    throw new NotImplementedException("Нет отработки для обчных акций");
                }
            }

            var dm = new DiversificationModel(countriesToPart.OrderByDescending(e => e.Value).ToList(),
                sectorToPart.OrderByDescending(e => e.Value).ToList(),
                industryToPart.OrderByDescending(e => e.Value).ToList(),
                isinToPart.OrderByDescending(e => e.Value).ToList(),
                isinToTicker.ToArray(),
                isinToName.ToArray());

            var total = dm.IsinToPart.Sum(pair => pair.Value);

            return dm;
        }
    }

    public static class ListExtensions
    {
        public static void AddingWithoutDublicates(this List<KeyValuePair<string, double?>> list, KeyValuePair<string, double?> valuePair)
        {
            if (list.Any(e => e.Key == valuePair.Key))
            {
                var index = list.IndexOf(list.First(e => e.Key == valuePair.Key));
                list[index] = new KeyValuePair<string, double?>(list[index].Key, list[index].Value + valuePair.Value);
            }
            else
                list.Add(valuePair);
        }
    }
}
