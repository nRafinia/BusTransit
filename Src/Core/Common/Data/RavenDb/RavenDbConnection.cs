using Common.Tools;
using Raven.Client.Documents;

namespace Common.Data.RavenDb
{
    public class RavenDbConnection : IRavenDbConnection
    {
        private static IDocumentStore _connection = null;
        public IDocumentStore Connection => _connection;

        public RavenDbConnection(IAppSetting appSetting)
        {
            if (_connection != null)
                return;

            var config = appSetting.Get<RavenDbConnectionModel>("DbConnection");

            _connection = new DocumentStore()
            {
                Urls = config.Servers,
                Database = config.Database,
                Conventions = { }
            };

            _connection.Initialize();
        }

        public void Dispose()
        {
            Connection?.Dispose();
        }
    }
}