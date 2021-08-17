using AkaData;
using AkaDB.MySql;
using AkaUtility;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public class ItemDynamicQuest : Item
    {
        private uint _dynamicQuestGroupId;
        private uint _containerQuestGroupId;
        public ItemDynamicQuest(uint userId, uint dynamicQuestGroupId, int containerQuestGroupId, DBContext db) : base(userId, 0, db)
        {
            _dynamicQuestGroupId = dynamicQuestGroupId;
            _containerQuestGroupId = (uint)containerQuestGroupId;
        }

        public override async Task Get(string logCategory)
        {
            var query = new StringBuilder();
            query.Append("UPDATE quests SET dynamicQuestId = ").Append(_dynamicQuestGroupId)
                 .Append(", performCount = 0, receivedOrder = 0, completedOrder = 0" +
                         " WHERE userId = ").Append(_userId)
                         .Append(" AND id = ").Append(_containerQuestGroupId)
                 .Append(";");
            await _db.ExecuteNonQueryAsync(query.ToString());
        }
    }
}
