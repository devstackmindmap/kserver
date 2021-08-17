using System;
using System.Data.Common;
using System.Threading.Tasks;
using AkaDB.MySql;

namespace Common.Entities.Item
{
    public class Energy : Item
    {
        public Energy(uint userId, int count, DBContext db) : base(userId, count, db)
        {
        }

        public override async Task Get(string logCategory)
        {
            AkaLogger.Log.Item.EnergyGet(_strUserId, _strCount, logCategory);
        }
    }
}
