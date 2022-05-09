using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public class FinexFactory : IEtfAbstractFactory
    {
        private IEtfNamesProvider _etfNamesProvider;
        private IEtfDataInitialisator _etfDataInitialisator;
        //private IEtfCostProvider _etfCostProvider;
        //private IEtfCompositionDataProvider _etfCompositionDataProvider;
        private ICurrencyConverter _currencyConverter;
        private IShareDataProvider _shareDataProvider;
        public FinexFactory()
        {
            _etfNamesProvider = new FinexEtfNamesWebProvider();
            _currencyConverter = new CurrencyConverter(new InvestingCurrencyExchangeRateProvider());
            _etfDataInitialisator = new InvestingEtfDataInitialisator(new FinexEtfCompositionDataProvider(), 
                new InvestingEtfCostProvider(_currencyConverter));
            _shareDataProvider = new InvestingShareDataProvider(_currencyConverter);
        }
        public FinexFactory(IEtfNamesProvider? etfNamesProvider,
            IEtfDataInitialisator? etfDataInitialisator,
            IShareDataProvider? shareDataProvider)
        {
            _etfNamesProvider = etfNamesProvider ?? new FinexEtfNamesWebProvider();
            _currencyConverter = new CurrencyConverter(new InvestingCurrencyExchangeRateProvider());
            _etfDataInitialisator = etfDataInitialisator ?? new InvestingEtfDataInitialisator(new FinexEtfCompositionDataProvider(),
                new InvestingEtfCostProvider(_currencyConverter));
            _shareDataProvider = shareDataProvider ?? new InvestingShareDataProvider(_currencyConverter);
        }
        public IEtfProviderData CreateEtfProviderData()
            => new FinexData();
        public void InitialiseEtfProviderData(IEtfProviderData finexData)
        {
            if (finexData is not FinexData)
                throw new ArgumentException("finexData should be FinexData");

            List<string> etfNames = _etfNamesProvider.GetEtfNames();
            List<IEtfData> etfDatas = new List<IEtfData>();
            foreach (var etfName in etfNames)
                etfDatas.Add(this.CreateEtfData(etfName));
            finexData.Initialise(etfNames, etfDatas);
        }
        public IEtfData CreateEtfData(string etfName)
            => new FinexEtfData(etfName);
        public void InitialiseEtfData(IEtfData finexEtfData)
        {
            if(finexEtfData is not FinexEtfData)
                throw new ArgumentException("finexEtfData should be FinexEtfData");

            _etfDataInitialisator.InitialiseEtfData(finexEtfData);
            _etfDataInitialisator.InitialiseEtfData(finexEtfData);
        }
        public IShareData CreateShareData(string shareName, string isin)
            => new ShareData(shareName, isin, null);
        public void InitialiseShareData(IShareData shareData)
        {
            if (shareData.Isin == null)
                throw new NullReferenceException("Isin was null");
            try
            {
                IShareData? initialisedShareData = _shareDataProvider.GetShareDataByIsin(shareData.Isin);
                shareData.Initialise(initialisedShareData.Ticker,
                    initialisedShareData.Country,
                    initialisedShareData.Sector,
                    initialisedShareData.Industry,
                    shareData.Isin,
                    shareData.PartInEtf,
                    initialisedShareData.Price);
            }
            catch(NoSearchResultException e)
            {
                shareData.Initialise(null, null, null, null, shareData.Isin, shareData.PartInEtf, null);
            }
            catch (BondException e)
            {
                shareData.Initialise(null, null, null, null, shareData.Isin, shareData.PartInEtf, null);
            }
        }
    }
}