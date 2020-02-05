using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Data.Dapper
{
    public class DapperConnectionConfig: DbConnectionModel
    {
        public string ConnectionString { get; set; }
    }
}
