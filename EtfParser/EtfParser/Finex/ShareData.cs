using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public class ShareData : IShareData
    {
        public string ShareName { get; }
        public string? Ticker { get; private set; }
        public string? Country { get; private set; }
        public string? Sector { get; private set; }
        public string? Industry { get; private set; }
        public string? Isin { get; private set; }
        public double? PartInEtf { get; set; }
        public double? Price { get; private set; }
        public bool IsInitialised { get; private set; }
        public ShareData(string shareName)
        {
            ShareName = shareName;
            IsInitialised = false;
        }
        public ShareData(string shareName, string isin, double? partInEtf)
        {
            ShareName = shareName;
            Isin = isin;
            PartInEtf = partInEtf;
            IsInitialised = false;
        }
        public ShareData(string shareName,
            string? isin, double? partInEtf,
            string? ticker, string? country, 
            string? sector, string? industry, 
            double? price, bool isInitialised) : this(shareName)
        {
            Initialise(ticker, country, sector, industry, isin, partInEtf, price);
        }

        public void Initialise(string? ticker, string? country,
            string? sector, string? industry,
            string? isin, double? partInEtf,
            double? price)
        {
            Ticker = ticker;
            Country = country;
            Sector = sector;
            Industry = industry;
            Isin = isin;
            PartInEtf = partInEtf;
            Price = price;
            IsInitialised = true;
        }
    }
}