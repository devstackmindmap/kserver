using AkaData;
using AkaDB.MySql;
using AkaEnum;
using Common;
using Common.Entities.Charger;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLogic.User.DataUpdator
{
    public class QuestUpdator : UserInitDataUpdator
    {
        internal QuestUpdator(uint userId, DBContext db, IEnumerable<uint> updateIdList) : base(userId, db, updateIdList)
        {
        }

        public override async Task Run()
        {
            string targetTable = TableName.QUEST;
            ICollection<uint> dataCsv = Data.GetAllQuests().Keys; ;
            string insertColumns = "(userId, id, completedOrder, questType)";
            string insertValues = 
                string.Join(",", 
                        UpdateIdList.Where(dataCsv.Contains)
                                    .Select(id =>
                                    {
                                        var firstQuest = Data.GetQuest(id).First();
                                        return $"({StrUserId},{id.ToString()}, {(firstQuest.QuestProcessType == QuestProcessType.Completed ? "1" : "0")},{ ((int)firstQuest.QuestType).ToString() })";
                                    }));

            await ExecuteInsertQuery(targetTable, insertColumns, insertValues);
        }
    }
}
