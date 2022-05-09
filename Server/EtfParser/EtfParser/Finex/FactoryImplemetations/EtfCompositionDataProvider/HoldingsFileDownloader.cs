using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public class HoldingsFileDownloader
    {
        private readonly string _savePath;
        private string DownloadUrlTemplate(string ticker)
            => string.Format("https://cdn.finex-etf.ru/documents/{0}/holding_info/RU/holdings_{0}.pdf", ticker);

        public HoldingsFileDownloader()
        {
            _savePath = "temp/";
        }
        public HoldingsFileDownloader(string savePath)
        {
            _savePath = savePath;
        }
        public void ClearDirAndDownloadHoldingsFiles(List<string> etfTickers)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(_savePath);
            if (directoryInfo.Exists)
                directoryInfo.Delete();
            directoryInfo.Create();
            foreach (var ticker in etfTickers)
                DownloadHoldingsFile(ticker);
        }
        public void DownloadHoldingsFile(string etfTicker, bool onlyIfNotDownloaded = false)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(_savePath);
            if (!directoryInfo.Exists)
                directoryInfo.Create();

            bool isHoldingFileDownloaded = directoryInfo.EnumerateFiles().Any(fileInfo => fileInfo.Name == etfTicker + ".pdf");

            Thread.Sleep(100);
            if (isHoldingFileDownloaded)
            {
                if (onlyIfNotDownloaded)
                    return;
                else
                    File.Delete(_savePath + etfTicker + ".pdf");
            }
            Thread.Sleep(100);
            new WebClient().DownloadFile(DownloadUrlTemplate(etfTicker), _savePath + etfTicker + ".pdf");
        }

    }
}
