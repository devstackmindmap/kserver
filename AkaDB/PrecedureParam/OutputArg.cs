using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace AkaDB.MySql
{
    public class OutputArg
    {
        public string Key { get; set; }
        public MySqlDbType ValueType { get; set; }

        public OutputArg(string key, MySqlDbType dbType)
        {
            Key = key;
            ValueType = dbType;
        }
    }
}
