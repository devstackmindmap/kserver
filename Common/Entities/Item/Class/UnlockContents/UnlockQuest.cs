using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using AkaUtility;
using Common.Entities.Stage;
using Common.Quest;
using System;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public class UnlockQuest : Item, IUnlockContents
    {
        private uint _questGroupId;

        public UnlockQuest(uint userId, uint classId, DBContext db) : base(userId, 0, db)
        {
            _questGroupId = classId;
        }

        public override async Task Get(string logCategory)
        {
            await Update(logCategory);
        }


        private async Task<int> Update(string logCategory)
        {
            var firstQuest = Data.GetQuest(_questGroupId).First();

            var query = new StringBuilder();
            query.Append("INSERT IGNORE INTO quests(userId, id, completedOrder, questType) VALUES(")
                 .Append(string.Join(",", _strUserId, firstQuest.QuestGroupId, 0, (int)firstQuest.QuestType))
                 .Append(");");
            if (await _db.ExecuteNonQueryAsync(query.ToString()) > 0)
            {
                Log.Item.QuestUnlock(_userId, _questGroupId, logCategory);
                return 1;
            }
            return 0;
        }

        public async Task<bool> IsHave()
        {

            var query = new StringBuilder();
            query.Append("SELECT id  FROM quests WHERE userId =")
                 .Append(_strUserId)
                 .Append(" AND id = ")
                 .Append(_questGroupId)
                 .Append(";");
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                return cursor.Read();
            }
        }

        public bool IsValidData()
        {
            return Data.GetQuest(_questGroupId)?.Any() == true;
        }
    }
}
