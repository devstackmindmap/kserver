using System.Linq;
using System.Threading.Tasks;
using AkaDB.MySql;
using Common.Entities.Stage;
using CommonProtocol;

namespace WebServer.Controller.Stage
{
    public class WebSetSaveDeck : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var protoSaveDeckInfo = requestInfo as ProtoSetSaveDeck;
            bool result = false;
            
            using (var db = new DBContext(protoSaveDeckInfo.UserId))
            {
                var stageInfo = new StageInfo(db, protoSaveDeckInfo.UserId, 0, protoSaveDeckInfo.BattleType);
                var stageLevelInfo = await stageInfo.GetRoguelikeStageInfo();

                var newCardStatId = protoSaveDeckInfo.NewCardStatId == 0 ? new uint[] { } : new uint[] { protoSaveDeckInfo.NewCardStatId };
                var oldCardStatId = protoSaveDeckInfo.OldCardStatId == 0 ? new uint[] { } : new uint[] { protoSaveDeckInfo.OldCardStatId };
                var newTreasureId = protoSaveDeckInfo.NewTreasureId == 0 ? new uint[] { } : new uint[] { protoSaveDeckInfo.NewTreasureId };

                
                if (oldCardStatId.Count() == newCardStatId.Count()
                    && newCardStatId.All(stageLevelInfo.ReplaceCardStatIdList.Contains)
                    && oldCardStatId.All(stageLevelInfo.CardStatIdList.Contains)
                    && newTreasureId.All(stageLevelInfo.ProposalTreasureIdList.Contains)
                )
                {
                    var userId = protoSaveDeckInfo.UserId.ToString();
                    var battleType = ((int)protoSaveDeckInfo.BattleType).ToString();

                    var newTreasureIdList = stageLevelInfo.TreasureIdList.Concat(newTreasureId);
                    if (oldCardStatId.Any())
                        stageLevelInfo.CardStatIdList.RemoveAt(stageLevelInfo.CardStatIdList.IndexOf(oldCardStatId.First()));
                    var newCardStatIdList = stageLevelInfo.CardStatIdList.Concat(newCardStatId);

                    var updateCardStatIdList = string.Join("/", newCardStatIdList);
                    var treasureIdList = string.Join("/", newTreasureIdList);

                    var sql = $"UPDATE inprogress_stage SET cardStatIdList = '{updateCardStatIdList}', replaceCardStatIdList = '' ,treasureIdList = '{treasureIdList}', proposalTreasureIdList = ''  "
                             + $" WHERE userId = {userId} AND battleType = {battleType};";

                    result = 1 == await db.ExecuteNonQueryAsync(sql);
                }
 ;
            }
               
            return new ProtoOnSetSaveDeck { Result = result };
        }

    }
   
}
