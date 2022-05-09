using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private ICurrensyExchangeRateProvider _exchangeRateProvider;
        private Dictionary<string, Dictionary<string, double>> _exchangeRates;
        public CurrencyConverter(ICurrensyExchangeRateProvider currensyExchangeRateProvider)
        {
            _exchangeRateProvider = currensyExchangeRateProvider;
            _exchangeRates = new Dictionary<string, Dictionary<string, double>>();
        }
        public double ConvertFromOneCurrencyToAnother(string firstCurrency, string secondCurrency, double amountInFirstCurrency)
        {
            if (!_exchangeRates.ContainsKey(firstCurrency) || !_exchangeRates[firstCurrency].ContainsKey(secondCurrency))
                AddCurrencyExchangeRate(firstCurrency, secondCurrency);
            return _exchangeRates[firstCurrency][secondCurrency] * amountInFirstCurrency;
        }
        private void AddCurrencyExchangeRate(string firstCurrency, string secondCurrency)
        {
            if (!_exchangeRates.ContainsKey(firstCurrency))
                _exchangeRates[firstCurrency] = new Dictionary<string, double>();
            if (_exchangeRates[firstCurrency].ContainsKey(secondCurrency))
                throw new ArgumentException("Already Exists");
            _exchangeRates[firstCurrency][secondCurrency] = _exchangeRateProvider.GetCurrencyRate(firstCurrency, secondCurrency);
        }
    }
}
