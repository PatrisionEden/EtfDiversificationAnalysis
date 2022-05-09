using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public class FinexEtfNamesLocalProvider : IEtfNamesProvider
    {
        private string _path = "/temp";
        public FinexEtfNamesLocalProvider()
        {
        }
        public FinexEtfNamesLocalProvider(string pathToDir)
        {
            _path = pathToDir;
        }
        public List<string> GetEtfNames()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(_path);
            return directoryInfo.EnumerateFiles().Select(fi => fi.Name.Split('.').First()).ToList();
        }
    }
}
