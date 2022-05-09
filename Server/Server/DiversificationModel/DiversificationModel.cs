namespace Server
{
    public class DiversificationModel
    {
        public List<KeyValuePair<string, double?>> CountriesToPart;
        public List<KeyValuePair<string, double?>> SectorToPart;
        public List<KeyValuePair<string, double?>> IndustryToPart;
        //public List<Security> Securities;
        public List<KeyValuePair<string, double?>> IsinToPart;

        public DiversificationModel(List<KeyValuePair<string, double?>> countriesToPart, 
            List<KeyValuePair<string, double?>> sectorToPart, List<KeyValuePair<string, double?>> industryToPart,
            //List<Security> securities,
            List<KeyValuePair<string, double?>> isinToPart)
        {
            CountriesToPart = countriesToPart;
            SectorToPart = sectorToPart;
            IndustryToPart = industryToPart;
            //Securities = securities;
            IsinToPart = isinToPart;
        }

        public static DiversificationModel Create(string login)
        {
            List<KeyValuePair<string, double?>> countriesToPart = new List<KeyValuePair<string, double?>>();
            List<KeyValuePair<string, double?>> sectorToPart = new List<KeyValuePair<string, double?>>();
            List<KeyValuePair<string, double?>> industryToPart = new List<KeyValuePair<string, double?>>();
            //List<KeyValuePair<string, double?>> isinToPart = new List<Security>();
            List<KeyValuePair<string, double?>> isinToPart = new List<KeyValuePair<string, double?>>();

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
                        if(partInPortfolio == 0)
                        {
                            ;
                        }

                        countriesToPart.AddingWithoutDublicates(new KeyValuePair<string, double?>(share.Country, partInPortfolio));
                        sectorToPart.AddingWithoutDublicates(new KeyValuePair<string, double?>(share.Sector, partInPortfolio));
                        industryToPart.AddingWithoutDublicates(new KeyValuePair<string, double?>(share.Industry, partInPortfolio));
                        isinToPart.AddingWithoutDublicates(new KeyValuePair<string, double?>(share.Isin, partInPortfolio));
                    }
                }
                else
                {
                    throw new NotImplementedException("Нет отработки для обчных акций");
                }
            }

            return new DiversificationModel(countriesToPart.OrderByDescending(e => e.Value).ToList(),
                sectorToPart.OrderByDescending(e => e.Value).ToList(),
                industryToPart.OrderByDescending(e => e.Value).ToList(),
                isinToPart.OrderByDescending(e => e.Value).ToList());
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
