using AkaDB.MySql;
using KnightUWP.Dao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;

namespace KnightUWP.Servicecs
{
    public static class TODO
    {
        public static string[] Battle()
        {
            return new string[] {
                "random deck setting",
                "unit add",
                "skill add",
                "levelup",
                "enemy death check",
            };
        }
    }
}
