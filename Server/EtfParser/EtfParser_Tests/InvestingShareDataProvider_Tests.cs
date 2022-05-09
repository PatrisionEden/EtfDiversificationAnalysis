using NUnit.Framework;
using EtfParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser_Tests
{
    [TestFixture]
    public class InvestingShareDataProvider_Tests
    {
        [Test]
        public void GetShareDataByIsin_GetYNDX_IsInitialised()
        {
            InvestingShareDataProvider investingShareDataProvider = new InvestingShareDataProvider();

            var share = investingShareDataProvider.GetShareDataByIsin("RU0009024277");

            Assert.IsTrue(share.IsInitialised);
            Assert.IsTrue(true);
        }
    }
}
