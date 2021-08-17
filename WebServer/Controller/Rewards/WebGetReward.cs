using System.Collections.Generic;
using System.Threading.Tasks;
using AkaDB.MySql;
using AkaEnum;
using Common.Entities.SquareObject;
using Common.Quest;
using CommonProtocol;

namespace WebServer.Controller.Rewards
{
    public class WebGetReward : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoGetReward;
            var res = new ProtoOnGetReward
            {
                ItemResults = new List<ProtoItemResult>()
            };

            using (var db = new DBContext(req.UserId))
            {
                await db.BeginTransactionCallback(async () =>
                {
                    (RewardResultType result, List<ProtoItemResult> rewardItems) result;
                    switch (req.GettingRewardType)
                    {
                        case GettingRewardType.QuestReward:
                            result = await new QuestIO(req.UserId, db).GetRewards(req.ClassIdWithItemValueList);
                            res.ItemResults = result.rewardItems;
                            res.Result = result.result;
                            break;
                        case GettingRewardType.SquareObject:
                            result = await new SquareObjectIO(db, null).GetRewards(req.UserId);
                            res.ItemResults = result.rewardItems;
                            res.Result = result.result;
                            break;
                        default:
                            break;
                    }

                    return true;
                });
            }

            return res;
        }
    }
}
