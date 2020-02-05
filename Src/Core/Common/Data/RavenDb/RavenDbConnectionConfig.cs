namespace Common.Data.RavenDb
{
    public class RavenDbConnectionConfig: DbConnectionModel
    {
        public string[] Servers { get; set; }
        public string DatabaseName { get; set; }
    }
}