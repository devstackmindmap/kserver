using AkaDB.MySql;
using Common.Entities.Item;
using CommonProtocol;
using System.Threading.Tasks;
using AkaEnum;
using System.Linq;
using AkaData;
using Common;
using System.Collections.Generic;
using System.Text;
using AkaLogger;

namespace WebServer.Controller.Quest
{
    public class WebNewQuest : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoNewQuest;
            var res = new ProtoOnNewQuest() {  ResultType = ResultType.Fail };

            using (var db = new DBContext(req.UserId))
            {
                await db.BeginTransactionCallback(async () =>
                {
                    var questItem = CreateSelectableContents(ItemType.DynamicQuest, req, db);
                    if (questItem != null)
                        res = await questItem.CreateNew();
                    return true;
                });
            }

            return res;
        }


        public static DailyQuest CreateSelectableContents(ItemType itemType, ProtoNewQuest newQuestInfo, DBContext db)
        {
            //TODO 로그라이크 refactoring
            switch (itemType)
            {
                case ItemType.DynamicQuest when newQuestInfo.QuestType == QuestType.Daily:
                    return new DailyQuest(newQuestInfo.UserId, newQuestInfo.TargetQuestGroupId, db);
                default:
                    return null;
            }

        }

    }
}
