using AkaDB.MySql;
using AkaEnum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public class EventCoin : TermMaterial
    {
        public EventCoin(uint userId, DBContext accountDb, DBContext userDb, int count = 0) 
            : base(userId, count, ColumnName.EVENT_COIN, MaterialType.EventCoin, TermMaterialType.EventCoin, accountDb, userDb)
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
