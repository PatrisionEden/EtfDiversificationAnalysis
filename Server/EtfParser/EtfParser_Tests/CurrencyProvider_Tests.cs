using NUnit.Framework;
using EtfParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace EtfParser_Tests
{
    [TestFixture]
    public class CurrencyProvider_Tests
    {
        [Test]
        public void ConvertFromOneCurrencyToAnother_ExchageRate100_Returns100()
        {
            Mock<ICurrensyExchangeRateProvider> stub = new Mock<ICurrensyExchangeRateProvider>();
            stub.Setup(rp => rp.GetCurrencyRate(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((dc, sc) => 100);
            CurrencyConverter currencyConverter = new CurrencyConverter(stub.Object);

            double lols = 1;
            double keks = currencyConverter.ConvertFromOneCurrencyToAnother("lol", "kek", lols);

            Assert.AreEqual(keks, 100, 0.00001);
        }
        [Test]
        public void ConvertFromOneCurrencyToAnother_ExchageRate100_ExchangeRateProviderCalssOnlyOnce()
        {
            Mock<ICurrensyExchangeRateProvider> stub = new Mock<ICurrensyExchangeRateProvider>();
            stub.Setup(rp => rp.GetCurrencyRate(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((dc, sc) => 100);
            CurrencyConverter currencyConverter = new CurrencyConverter(stub.Object);

            double lols = 1;
            double keks = currencyConverter.ConvertFromOneCurrencyToAnother("lol", "kek", lols);
            double lols_10 = 10;
            double keks_10 = currencyConverter.ConvertFromOneCurrencyToAnother("lol", "kek", lols_10);

            Assert.AreEqual(keks, 100, 0.00001);
            Assert.AreEqual(keks_10, 1000, 0.0001);
            stub.Verify(rp => rp.GetCurrencyRate(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
        [Test]
        public void ConvertFromOneCurrencyToAnother_FromUsdToRubAndBackwards_WithoutBigEpsilon()
        {
            CurrencyConverter currencyConverter = new CurrencyConverter(new InvestingCurrencyExchangeRateProvider());

            double oneDollar = 1;
            double oneDollarInRub = currencyConverter.ConvertFromOneCurrencyToAnother("usd", "rub", oneDollar);
            double oneDollarInRubInUsd = currencyConverter.ConvertFromOneCurrencyToAnother("rub", "usd", oneDollarInRub);

            Assert.AreEqual(oneDollar, oneDollarInRubInUsd, 0.001);
        }
    }
}
