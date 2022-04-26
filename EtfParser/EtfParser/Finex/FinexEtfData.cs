using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public class FinexEtfData : IEtfData
    {
        public string EtfName { get; }
        public string Ticker
            => EtfName;
        public double? Cost { get; private set; }
        public List<IShareData>? ShareDatas { get; private set; }
        public bool IsInitialised { get; private set; }

        public Dictionary<string, double>? IsinToPartInEtf { get; private set; }

        public FinexEtfData(string etfName)
        {
            EtfName = etfName;
            IsInitialised = false;
        }
        public void Initialise(double? cost, List<IShareData>? shareDatas, Dictionary<string, double> isinToPartInEtf)
        {
            Cost = cost;
            ShareDatas = shareDatas;
            IsInitialised = true;
            IsinToPartInEtf = isinToPartInEtf;
        }
    }
}
