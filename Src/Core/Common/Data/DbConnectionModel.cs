namespace Common.Data
{
    public class DbConnectionModel
    {
        public string Database { get; set; }
        
        private DatabaseType? _databaseType = null;
        internal DatabaseType DatabaseType
        {
            get
            {
                if (_databaseType != null)
                    return _databaseType.Value;

                _databaseType = Database.ToLower() switch
                {
                    "1" => DatabaseType.RavenDB,
                    "ravendb" => DatabaseType.RavenDB,

                    "2" => DatabaseType.SQLServer,
                    "sqlserver" => DatabaseType.SQLServer,

                    "3" => DatabaseType.PostgreSQL,
                    "postgresql" => DatabaseType.PostgreSQL,

                    "4" => DatabaseType.SQLite,
                    "sqlite" => DatabaseType.SQLite,

                    "5" => DatabaseType.MySQL,
                    "mysql" => DatabaseType.MySQL,

                    "6" => DatabaseType.LiteDB,
                    "litedb" => DatabaseType.LiteDB,

                    _ => DatabaseType.None
                };

                return _databaseType.Value;
            }
        }
    }

}