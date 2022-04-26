using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public class NoSearchResultException : Exception
    {
        public NoSearchResultException(string message) : base(message) { }
    }
    public class BondException : Exception
    {
        public BondException(string message) : base(message) { }
    }
    public class EtfException : Exception
    {
        public EtfException(string message) : base(message) { }
    }
    public class CurrencyParseException : Exception
    {
        public CurrencyParseException(string message) : base(message) { }
    }

}
