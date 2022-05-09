using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class DBConnection
    {
        private string _dbFileName;
        private SQLiteConnection _dbConnection;
        public DBConnection(string dbFileName)
        {
            _dbFileName = dbFileName;
            _dbConnection = 
                new SQLiteConnection(DBConnectionInitialisationStringTemplate(dbFileName));
            _dbConnection.Open();
        }
        public DataTable GetTableFromSql(string sql)
        {
            // Создаем объект DataAdapter
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, _dbConnection);
            // Создаем объект Dataset
            DataSet ds = new DataSet();
            // Заполняем Dataset
            adapter.Fill(ds);
            // Отображаем данные
            return ds.Tables[0];
        }

        public void ExecuteSqlCommand(string sql)
        {
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }
        private string DBConnectionInitialisationStringTemplate(string dbFileName)
            => string.Format("Data Source={0};", dbFileName);
    }
}
