using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public interface IEtfAbstractFactory
    {
        public IEtfProviderData CreateEtfProviderData();
        public void InitialiseEtfProviderData(IEtfProviderData etfProviderData);
        public IEtfData CreateEtfData(string etfName);
        public IShareData CreateShareData(string shareName, string isin);
    }
}
