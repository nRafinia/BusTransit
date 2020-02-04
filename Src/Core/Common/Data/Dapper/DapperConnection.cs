using System.Data;
using System.Data.SqlClient;
using Common.Tools;

namespace Common.Data.Dapper
{
    public class DapperConnection : IDapperConnection
    {
        public IDbConnection Connection { get; }

        public DapperConnection(IAppSetting appSetting)
        {
            var config = appSetting.Get<DapperConnectionModel>("DbConnection");
            Connection = new SqlConnection(config.ConnectionString);
        }

        public void Dispose()
        {
            Connection?.Dispose();
        }
    }
}