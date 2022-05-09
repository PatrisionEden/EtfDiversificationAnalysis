using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public interface IEtfData
    {
        public string EtfName { get; }
        public string Ticker { get; }
        public double? Cost { get; }
        public string? Isin { get; }
        public List<IShareData>? ShareDatas { get; }
        public Dictionary<string, double>? IsinToPartInEtf { get; }
        public bool IsInitialised { get; }
        public void Initialise(double? cost, List<IShareData>? shareDatas, Dictionary<string, double> isinToPartInEtf, string isin);
    }
}
