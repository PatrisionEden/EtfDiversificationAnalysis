using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public interface IEtfDataInitialisator
    {
        public void InitialiseEtfData(IEtfData etfData);
    }
}
