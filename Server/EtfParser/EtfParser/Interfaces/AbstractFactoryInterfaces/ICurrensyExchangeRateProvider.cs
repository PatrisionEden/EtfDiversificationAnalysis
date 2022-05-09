using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public interface ICurrensyExchangeRateProvider
    {
        public double GetCurrencyRate(string firstCurrency, string secondCurrency);
    }
}
