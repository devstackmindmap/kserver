using AkaDB.MySql;
using AkaEnum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public class Gold : Material
    {
        public Gold(uint userId, DBContext db, int count = 0) : base(userId, count, ColumnName.USER_GOLD, MaterialType.Gold, db)
        {
        }

        public override async Task Get(string logCategory)
        {
            await base.Get(logCategory);
            if (_count == 0)
                return;

        }
    }
}
