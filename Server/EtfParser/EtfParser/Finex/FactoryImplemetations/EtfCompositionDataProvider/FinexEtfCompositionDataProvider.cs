using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public class FinexEtfCompositionDataProvider : IEtfCompositionDataProvider
    {
        private HoldingsFileDownloader _holdingsFileDownloader;
        private FinexEtfHoldingsFileParser _finexEtfHoldingsFileParser;
        private const string _savePath = "temp/";
        public FinexEtfCompositionDataProvider(HoldingsFileDownloader holdingsFileDownloader)
        {
            _holdingsFileDownloader = holdingsFileDownloader;
            _finexEtfHoldingsFileParser = new();
        }
        public FinexEtfCompositionDataProvider()
        {
            _holdingsFileDownloader = new HoldingsFileDownloader();
            _finexEtfHoldingsFileParser = new();
        }
        public IEnumerable<IShareData>? GetComposition(string etfTicker)
        {
            _holdingsFileDownloader.DownloadHoldingsFile(etfTicker, false);
            var tuples = _finexEtfHoldingsFileParser.ParseHoldingsFile(_savePath + etfTicker + ".pdf");
            foreach (var tuple in tuples)
                yield return new ShareData(tuple.ShareName.Trim(), tuple.Isin, tuple.PartInEtf);
        }
    }
}
