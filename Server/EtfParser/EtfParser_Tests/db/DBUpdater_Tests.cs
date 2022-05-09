using NUnit.Framework;
using EtfParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser_Tests.db
{
    [TestFixture]
    public class DBUpdater_Tests
    {
        [Test]
        public void SaveOrUpdateIEtfProviderData_OK()
        {
            DBUpdater dBUpdater = new DBUpdater("../../../../db.db");
            var factory = new FinexFactory();
            var etfProviderData = factory.CreateEtfProviderData();
            factory.InitialiseEtfProviderData(etfProviderData);

            dBUpdater.SaveOrUpdateIEtfProviderData(etfProviderData);

            Assert.IsTrue(true);
        }
        [Test]
        public void LoadEtfProviderDataFromDB_OK()
        {
            DBUpdater dBUpdater = new DBUpdater("../../../../db.db");

            dBUpdater.LoadEtfProviderDataFromDB("FinEx");
        }
        [Test]
        public void SaveOrUpdateIEtfData_OK()
        {
            DBUpdater dBUpdater = new DBUpdater("../../../../tralala.db");

            var factory = new FinexFactory();
            var etfData = factory.CreateEtfData("FXES");
            factory.InitialiseEtfData(etfData);


            dBUpdater.SaveOrUpdateIEtfData(etfData);
        }
        [Test]
        public void LoadEtfDataFromDB_OK()
        {
            DBUpdater dBUpdater = new DBUpdater("../../../../db.db");

            IEtfData? some = dBUpdater.LoadEtfDataFromDB("FXES");
        }
        [Test]
        public void SaveOrUpdateIShareDataByIsin_OK()
        {
            DBUpdater dBUpdater = new DBUpdater("../../../../tralala.db");
            FinexFactory factory = new FinexFactory();
            var etf = factory.CreateEtfData("FXES");
            factory.InitialiseEtfData(etf);

            foreach (var share in etf.ShareDatas)
                dBUpdater.SaveOrUpdateIShareDataByIsin(share);
        }
        [Test]
        public void LoadShareDataFromDB_OK()
        {
            DBUpdater dBUpdater = new DBUpdater("../../../../tralala.db");

            var share = dBUpdater.LoadShareDataFromDB("JP3300200007");
        }
    }
}
