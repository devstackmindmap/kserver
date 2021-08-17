using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Item;
using CommonProtocol;
using Common.Quest;
using Common.Entities.Season;
using Common.Pass;

namespace WebServer.Controller.Quest
{
    public class WebGetQuest : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {            
            var req = requestInfo as ProtoGetQuestList;
            var res = new ProtoOnGetQuestList
            {
                QuestInfoList = new List<ProtoQuestInfo>()
            };

            using (var db = new DBContext(req.UserId))
            {
                uint currentSeasonPass = 0;
                using (var accountDb = new DBContext("AccountDBSetting"))
                {
                    var serverSeason = new ServerSeason(accountDb);
                    var seasonInfo = await serverSeason.GetSeasonPassInfo();
                    currentSeasonPass = seasonInfo.CurrentSeason;

                }

                await db.BeginTransactionCallback(async () =>
                {
                    SeasonPassManager seasonPassManager = new SeasonPassManager(req.UserId, currentSeasonPass, db);
                    await seasonPassManager.Update();
                    return true;
                });

                var questInfoList = await new QuestIO(req.UserId, currentSeasonPass, db).Select();
                res.QuestInfoList.AddRange(questInfoList);

            }
            return res;
        }
    }
}
