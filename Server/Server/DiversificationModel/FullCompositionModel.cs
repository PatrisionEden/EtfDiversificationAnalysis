using System.Diagnostics.CodeAnalysis;

namespace Server
{
    public class FullCompositionModel
    {
        public List<Share> Shares;
        public List<KeyValuePair<string, double?>> IsinToPart;

        private FullCompositionModel()
        {
            Shares = new();
            IsinToPart = new();
        }

        public static FullCompositionModel Create(string login)
        {
            FullCompositionModel fullCompositionModel = new FullCompositionModel();

            UserDataController userDataController = UserDataController.GetInstance();
            SecurityDataProvider securityDataProvider = SecurityDataProvider.GetInstance();

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
                    foreach (var compositionItem in composition)
                    {
                        Share share = securityDataProvider.GetShareByIsin(compositionItem.Key);
                        double partInEtf = compositionItem.Value;
                        double partInPortfolio = securityPartInPortfolio * partInEtf;
                        string ticker = share.Ticker;

                        if (partInPortfolio == 0)
                        {
                            ;
                        }

                        if(!fullCompositionModel.Shares.Any(s => s.Isin == share.Isin))
                            fullCompositionModel.Shares.Add(share);
                        fullCompositionModel.IsinToPart.AddingWithoutDublicates(new KeyValuePair<string, double?>(share.Isin, partInPortfolio));
                    }
                }
                else
                {
                    throw new NotImplementedException("Нет отработки для обчных акций");
                }
            }

            fullCompositionModel.Shares = fullCompositionModel.Shares
                .Distinct(new ShareComparer())
                .OrderByDescending(sh =>
                    fullCompositionModel.IsinToPart.FirstOrDefault(p => p.Key == sh.Isin).Value)
                .ToList();
            return fullCompositionModel;
        }
    }

    public class ShareComparer : IEqualityComparer<Share>
    {
        public bool Equals(Share? x, Share? y)
        {
            if (x == null || y == null)
                return false;
            if (x.Isin == y.Isin)
                return true;
            return false;
        }

        public int GetHashCode([DisallowNull] Share obj)
        {
            return obj.GetHashCode();
        }
    }
}