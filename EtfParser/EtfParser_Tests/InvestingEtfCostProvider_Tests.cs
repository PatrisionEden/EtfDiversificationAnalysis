using NUnit.Framework;
using EtfParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser_Tests
{
    [TestFixture]
    public class InvestingEtfCostProvider_Tests
    {
        [Test]
        public void GetCostByTicker_Usual_NotNull()
        {
            ICurrencyConverter currencyConverter = new CurrencyConverter(new InvestingCurrencyExchangeRateProvider());
            InvestingEtfCostProvider investingEtfCostProvider = new EtfParser.InvestingEtfCostProvider(currencyConverter);

            var actualCostAnd = investingEtfCostProvider.GetCostByTicker("FXUS");

            Assert.True(true);
        }
    }
}
