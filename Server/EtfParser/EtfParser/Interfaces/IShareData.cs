using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public interface IShareData
    {
        public string ShareName { get; }
        public string? Ticker { get; }
        public string? Country { get; }
        public string? Sector { get; }
        public string? Industry { get; }
        public string? Isin { get; }
        public double? PartInEtf { get; }
        public double? Price { get; }
        public bool IsInitialised { get; }
        public void Initialise(string? ticker, string? country,
            string? sector, string? industry,
            string? isin, double? partInEtf,
            double? price);
    }
}
