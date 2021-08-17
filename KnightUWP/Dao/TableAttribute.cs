using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightUWP.Dao
{
    public class TableAttribute : Attribute
    {
        public string Name;
        public string DB = "knightrun";
        public string DBContext = "UserDBSetting";
    }
}
