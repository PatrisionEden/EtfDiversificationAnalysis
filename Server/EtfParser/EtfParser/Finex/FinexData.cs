using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public class FinexData : IEtfProviderData
    {
        public string EtfProviderName
            => "FinEx";
        public List<string>? EtfNames { get; private set; }
        public List<IEtfData>? EtfDatas { get; private set; }
        public bool IsInitialised { get; private set; }
        public FinexData()
        {
            IsInitialised = false;
        }
        public void Initialise(List<string>? etfNames, List<IEtfData>? etfDatas)
        {
            EtfNames = etfNames;
            EtfDatas = etfDatas;
            IsInitialised = true;
        }
    }
}
