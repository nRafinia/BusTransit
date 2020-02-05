using System.Data;
using LiteDB;

namespace Common.Data.LiteDB
{
    public interface ILiteDbConnection : IBaseDbConnection<LiteDatabase>
    {

    }
}