using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public interface IEtfProviderData
    {
        public string EtfProviderName { get; }
        public List<string>? EtfNames { get; }
        public List<IEtfData>? EtfDatas { get; }
        public bool IsInitialised { get; }
        public void Initialise(List<string>? etfNames, List<IEtfData>? etfDatas);
    }
}
