using AkaDB.MySql;
using AkaEnum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public class EventBoxEnergy : TermMaterial
    {
        public EventBoxEnergy(uint userId, DBContext accountDb, DBContext userDb, int count = 0) 
            : base(userId, count, ColumnName.EVENT_COIN, MaterialType.EventBoxEnergy, TermMaterialType.EventBoxEnergy, accountDb, userDb)
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
