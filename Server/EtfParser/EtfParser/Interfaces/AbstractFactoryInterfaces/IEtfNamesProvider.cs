using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public interface IEtfNamesProvider
    {
        public List<string> GetEtfNames();
    }
}
