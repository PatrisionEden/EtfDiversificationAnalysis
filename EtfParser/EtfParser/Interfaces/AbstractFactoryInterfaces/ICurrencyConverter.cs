using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public interface ICurrencyConverter
    {
        public double ConvertFromOneCurrencyToAnother(string firstCurrency, string secondCurrency, double amountInFirstCurrency);
    }
}
