using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common.Entities.Item;
using Common.Quest;
using CommonProtocol;

namespace WebServer.Controller.Quest
{
    public class WebSetQuest : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoSetQuestList;
            var res = new ProtoOnSetQuestList
            {
                QuestInfoList = new  List<ProtoQuestInfo>()
            };
            
            using (var db = new DBContext(req.UserId))
            {
                var questIO = new QuestIO(req.UserId, db);
                var questList = req.QuestInfoList.Where(questInfo => (Data.GetQuest(questInfo.QuestGroupId)?.FirstOrDefault()?.QuestProcessType ?? QuestProcessType.None).In(QuestProcessType.Completed, QuestProcessType.ClientSide) );

                if (questList.Any())
                {
                    await db.BeginTransactionCallback(async () =>
                    {
                        res.QuestInfoList.AddRange(await questIO.UpdateQuests(false, questList));
                        return true;
                    });
                }
            }


            return res;
        }
    }
}
