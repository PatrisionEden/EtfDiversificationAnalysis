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
    public class EtfFactory_Tests
    {
        [Test]
        public void CreateEtfProviderData_Usual_WithoutExceptions()
        {
            FinexFactory factory = new FinexFactory();

            IEtfProviderData etfProvider = factory.CreateEtfProviderData();

            Assert.IsTrue(true);
        }

        [Test]
        public void CreateEtfProviderData_Usual_AreNotInitialised()
        {
            var factory = new FinexFactory();

            var etfProvider = factory.CreateEtfProviderData();

            Assert.AreEqual(null, etfProvider.EtfDatas);
            Assert.AreEqual(null, etfProvider.EtfNames);
            Assert.IsFalse(etfProvider.IsInitialised);

            Assert.IsTrue(true);
        }

        [Test]
        public void InitialiseEtfProviderData_Usual_AreInitialised()
        {
            FinexFactory factory = new FinexFactory();
            IEtfProviderData etfProvider = factory.CreateEtfProviderData();

            factory.InitialiseEtfProviderData(etfProvider);

            Assert.AreNotEqual(null, etfProvider.EtfDatas);
            Assert.AreNotEqual(null, etfProvider.EtfNames);
            Assert.IsTrue(etfProvider.IsInitialised);

            Assert.IsTrue(true);
        }
        [Test]
        public void CreateEtfData_Usual_WithoutExcpetions()
        {
            FinexFactory factory = new FinexFactory();

            factory.CreateEtfData("FXUS");

            Assert.IsTrue(true);
        }
        [Test]
        public void CreateEtfData_Usual_AreNotInitialised()
        {
            FinexFactory factory = new FinexFactory();

            IEtfData etfData = factory.CreateEtfData("FXUS");

            Assert.AreEqual(null, etfData.Cost);
            Assert.AreEqual(null, etfData.ShareDatas);
            Assert.IsFalse(etfData.IsInitialised);
        }
        [Test]
        public void InitialiseEtfData_Usual_AreInitialised()
        {
            FinexFactory factory = new FinexFactory();
            IEtfData etfData = factory.CreateEtfData("FXUS");

            factory.InitialiseEtfData(etfData);

            Assert.AreNotEqual(null, etfData.Cost);
            Assert.AreNotEqual(null, etfData.ShareDatas);
            Assert.IsTrue(etfData.IsInitialised);
        }
        [Test]
        public void Ambitious()
        {

            FinexFactory factory = new FinexFactory();
            IEtfData etfData = factory.CreateEtfData("FXES");

            factory.InitialiseEtfData(etfData);

            if (etfData.ShareDatas == null)
                Assert.Fail();
            foreach (var share in etfData.ShareDatas)
                factory.InitialiseShareData(share);

            Assert.IsTrue(etfData.IsInitialised);
        }
    }
}
