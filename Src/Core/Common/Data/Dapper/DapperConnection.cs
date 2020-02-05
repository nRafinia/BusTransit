using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using MySql.Data.MySqlClient;
using Npgsql;

namespace Common.Data.Dapper
{
    public class DapperConnection : IDapperConnection
    {
        public IDbConnection Connection { get; }

        public DapperConnection(/*IAppSetting appSetting*/ DbConnectionModel dbConnection)
        {
            //var config = appSetting.Get<DapperConnectionConfig>("DbConnection");
            var config = dbConnection as DapperConnectionConfig;

            switch (config.DatabaseType)
            {
                case DatabaseType.SQLServer:
                    Connection = new SqlConnection(config.ConnectionString);
                    break;
                case DatabaseType.PostgreSQL:
                    Connection = new NpgsqlConnection(config.ConnectionString);
                    break;
                case DatabaseType.SQLite:
                    Connection = new SQLiteConnection(config.ConnectionString);
                    break;
                case DatabaseType.MySQL:
                    Connection = new MySqlConnection(config.ConnectionString);
                    break;
            }
            //Connection = new SqlConnection(config.ConnectionString);
        }

        public void Dispose()
        {
            Connection?.Dispose();
        }
    }
}