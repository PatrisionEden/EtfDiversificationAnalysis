using EtfParser;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EtfParser_Tests
{
    [TestFixture]
    public class HoldingsFileDownloader_Test
    {
        [Test]
        public void ClearDirAndDownloadHoldingsFiles_SeveralEtfs_AreDownloaded()
        {
            string dirPath = "temp/";
            HoldingsFileDownloader holdingsFileDownloader = new HoldingsFileDownloader(dirPath);
            DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
            if (directoryInfo.Exists)
                directoryInfo.Delete(true);

            Assert.IsFalse(directoryInfo.Exists);
            holdingsFileDownloader.ClearDirAndDownloadHoldingsFiles(new() { "FXUS", "FXIM" });

            directoryInfo = new DirectoryInfo(dirPath);
            List<FileInfo> enumeratedFiles = directoryInfo.EnumerateFiles().ToList();
            Thread.Sleep(100);
            Assert.IsTrue(directoryInfo.Exists);
            Assert.AreEqual(2, enumeratedFiles.Count);
            foreach (var etfTicker in new List<string>(){ "FXUS", "FXIM" })
                enumeratedFiles.Any(fi => fi.Name.StartsWith(etfTicker));
        }

    }
}
