using Raven.Client.Documents;

namespace Common.Data.RavenDb
{
    public interface IRavenDbConnection : IBaseDbConnection<IDocumentStore>
    {

    }
}