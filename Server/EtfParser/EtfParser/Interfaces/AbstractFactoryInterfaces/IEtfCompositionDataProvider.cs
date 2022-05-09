using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public interface IEtfCompositionDataProvider
    {
        public IEnumerable<IShareData>? GetComposition(string etfTicker);
    }
}
