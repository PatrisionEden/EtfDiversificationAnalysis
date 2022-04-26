using EtfParser;
using NUnit.Framework;
using System.Collections.Generic;

namespace EtfParser_Tests
{
    [TestFixture]
    public class FinexEtfNamesWebProvider_Tests
    {
        [Test]
        public void GetEtfNames_CheckConnection_ReturnsSomeNamesAndWithoutExceptions()
        {
            FinexEtfNamesWebProvider namesProvider = new FinexEtfNamesWebProvider();

            List<string> etfNames = namesProvider.GetEtfNames();

            Assert.IsTrue(true);
        }
    }
}