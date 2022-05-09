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
    public class InvestingCurrencyExchangeRateProvider_Test
    {
        [Test]
        public void GetCurrencyRate_RubToUsd_Ok()
        {
            InvestingCurrencyExchangeRateProvider investingCurrencyExchangeProvider = new InvestingCurrencyExchangeRateProvider();

            double rate = investingCurrencyExchangeProvider.GetCurrencyRate("rub", "usd");

            Assert.IsTrue(true);
        }
    }
}
