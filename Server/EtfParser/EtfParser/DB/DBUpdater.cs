using Microsoft.Data.Sqlite;
using EtfParser.DB;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace EtfParser
{
    public class DBUpdater : IShareDataProvider, IEtfDataInitialisator 
    {
        private string _dbFileName;
        private DBConnection _dBConnection;

        public DBUpdater()
        {
            _dbFileName = "db.db";
            _dBConnection = new DBConnection(_dbFileName);
        }
        public DBUpdater(string dbFileName)
        {
            _dbFileName = dbFileName;
            _dBConnection = new DBConnection(_dbFileName);
        }

        public void SaveOrUpdateIEtfProviderData(IEtfProviderData etfProviderData, bool updateIfExists = true)
        {
            _dBConnection.ExecuteSqlCommand(Queries.CreateIfNotExistsEtfProvadersTable());
            if (IsEtfProviderDataRowExists(etfProviderData.EtfProviderName) && updateIfExists)
                _dBConnection.ExecuteSqlCommand(Queries.UpdateEtfProviderData(etfProviderData));
            else
                _dBConnection.ExecuteSqlCommand(Queries.InsertEtfProviderData(etfProviderData));
        }
        public IEtfProviderData LoadEtfProviderDataFromDB(string etfProviderName)
        {
            var row = _dBConnection.GetTableFromSql(Queries.GetEtfProviderData(etfProviderName)).Rows[0];
            if (row == null)
                throw new NullReferenceException();
            List<string>? etfNames = JsonConvert.DeserializeObject<List<string>?>(row[1].ToString() ?? "");
            //List<IEtfData>? etfDatas;
            bool isInitialised = (bool)row[3];
            throw new Exception();
        }
        public void SaveOrUpdateIEtfData(IEtfData etfData, bool updateIfExists = true)
        {
            _dBConnection.ExecuteSqlCommand(Queries.CreateIfNotExistsEtfsTable());
            if (IsEtfDataRowExists(etfData.EtfName) && updateIfExists)
                _dBConnection.ExecuteSqlCommand(Queries.UpdateEtfDataInEtfsTable(etfData));
            else
                _dBConnection.ExecuteSqlCommand(Queries.InsertEtfDataInEtfsTable(etfData));
        }
        public IEtfData LoadEtfDataFromDB(string etfName)
        {
            var row = _dBConnection.GetTableFromSql(Queries.GetEtfData(etfName)).Rows[0];
            if (row == null)
                throw new NullReferenceException();
            string ticker = row[1].ToString() ?? "";
            double? cost = double.Parse(row[2].ToString() ?? "");
            List<string>? shareIsinDatas = JsonConvert.DeserializeObject<List<string>?>(row[3].ToString() ?? "");
            List<IShareData>? shareDatas = shareIsinDatas?.Select(isin => LoadShareDataFromDB(isin)).ToList();
            Dictionary<string, double>? isinToPartInEtf = JsonConvert.DeserializeObject<Dictionary<string, double>?>(row[4].ToString() ?? "");
            string? isin = row[5].ToString() ?? "";
            bool isInitialised = (bool)row[6];
            //WARNING!!!!!!!!!!!!!!! IEtfData use FinexDataConstructor
            FinexEtfData finexEtfData = new FinexEtfData(etfName);
            if (isInitialised)
            {
                if (isinToPartInEtf == null)
                    throw new NullReferenceException("isinToPartInEtf was null");
                finexEtfData.Initialise(cost, shareDatas, isinToPartInEtf, isin);
            }
            return finexEtfData;
        }
        public void SaveOrUpdateIShareDataByIsin(IShareData shareData, bool updateIfExists = true)
        {
            _dBConnection.ExecuteSqlCommand(Queries.CreateIfNotExistsSharesTable());
            if (IsShareDataRowExists(shareData.Isin ?? "") && updateIfExists)
                _dBConnection.ExecuteSqlCommand(Queries.UpdateShareDataInSharesTableByIsin(shareData));
            else
                _dBConnection.ExecuteSqlCommand(Queries.InsertShareDataInSharesTable(shareData));
        }
        public IShareData LoadShareDataFromDB(string isin)
        {
            var row = _dBConnection.GetTableFromSql(Queries.GetShareDataByIsin(isin)).Rows[0];
            if (row == null)
                throw new NullReferenceException();
            string shareName = row[1].ToString() ?? "";
            string? ticker = row[2] is DBNull ? null : row[2].ToString();
            string? country = row[3] is DBNull ? null : row[3].ToString();
            string? sector = row[4] is DBNull ? null : row[4].ToString();
            string? industry = row[5] is DBNull ? null : row[5].ToString();
            double? price = row[6] is DBNull ? null : (double)row[6];
            bool isInitialised = (bool)row[7];

            return new ShareData(shareName, isin, null, ticker, country, sector, industry, price, isInitialised);
        }
        public IShareData GetShareDataByIsin(string ticker)
            => LoadShareDataFromDB(ticker);
        public void InitialiseEtfData(IEtfData etfData)
        {
            throw new NotImplementedException();
        }
        public bool IsEtfProviderDataRowExists(string etfProviderName)
            => ((long)_dBConnection.GetTableFromSql(
                Queries.CountOfRowsWithEtfProviderNameInEtfProvidersTable(
                    etfProviderName)).Rows[0][0] == 1);
        public bool IsEtfDataRowExists(string etfName)
            => ((long)_dBConnection.GetTableFromSql(
                Queries.CountOfRowsWithEtfNameInEtfsTable(
                    etfName)).Rows[0][0] == 1);
        public bool IsShareDataRowExists(string isin)
        {
            bool result;
            try
            {
                result = (long)_dBConnection.GetTableFromSql(
                    Queries.CountOfRowsWithIsinInSharesTable(
                        isin)).Rows[0][0] > 0;
            }
            catch(Exception e)
            {
                return false;
            }
            return result;
        }

        //System.Data.SQLite.SQLiteException: "SQL logic error
        //no such table: Shares"

    }
}
