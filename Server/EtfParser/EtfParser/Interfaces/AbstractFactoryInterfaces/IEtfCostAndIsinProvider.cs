using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public interface IEtfCostAndIsinProvider
    {
        public Tuple<double?, string?> GetCostAndIsinByTicker(string ticker);
    }
}
