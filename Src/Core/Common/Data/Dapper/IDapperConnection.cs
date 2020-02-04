using System.Data;
using Raven.Client.Documents;

namespace Common.Data.Dapper
{
    public interface IDapperConnection : IBaseDbConnection<IDbConnection>
    {

    }
}