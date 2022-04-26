using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public interface IEtfCostProvider
    {
        public double? GetCostByTicker(string ticker);
    }
}
