using Common.Tools;
using Raven.Client.Documents;

namespace Common.Data.RavenDb
{
    public class RavenDbConnection : IRavenDbConnection
    {
        public IDocumentStore Connection { get; }

        public RavenDbConnection(IAppSetting appSetting)
        {
            var config = appSetting.Get<DBConnectionModel>("DbConnection");

            Connection = new DocumentStore()
            {
                Urls = config.Servers ,
                Database = config.Database,
                Conventions = { }
            };

            Connection.Initialize();
        }

        public void Dispose()
        {
            Connection?.Dispose();
        }
    }
}