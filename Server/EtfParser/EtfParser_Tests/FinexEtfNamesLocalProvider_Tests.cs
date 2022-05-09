using System;
using NUnit;
using EtfParser;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace EtfParser_Tests
{
    [TestFixture]
    public class FinexEtfNamesLocalProvider_Tests
    {
        [Test]
        public void GetEtfNames_ItsReturns()
        {
            string pathToDirWithHoldingsFiles = "../../../temp";
            FinexEtfNamesLocalProvider namesProvider
                = new FinexEtfNamesLocalProvider(pathToDirWithHoldingsFiles);

            List<string> etfNames = namesProvider.GetEtfNames();

            Assert.IsTrue(true);
        }
    }
}
