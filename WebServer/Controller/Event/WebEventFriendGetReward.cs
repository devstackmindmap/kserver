using AkaData;
using AkaDB.MySql;
using Common.Entities.Reward;
using CommonProtocol;
using System;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Controller.Event
{
    public class WebEventFriendGetReward : BaseController
    {
        private StringBuilder _query = new StringBuilder();

        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserIdTargetId;
            var res = new ProtoEventGetReward();

            using (var userDb = new DBContext(req.UserId))
            {
                if (false == await IsValidRewardNum(userDb, req.UserId, req.TargetId))
                {
                    res.ResultType = AkaEnum.ResultType.Fail;
                    return res;
                }

                if (await IsAlreadyReceivedReward(userDb, req.UserId, req.TargetId))
                {
                    res.ResultType = AkaEnum.ResultType.AlreadyRewarded;
                    return res;
                }

                await userDb.BeginTransactionCallback(async () =>
                {
                    await AddReward(userDb, req.UserId, req.TargetId);
                    var friendEvent = Data.GetFriendEvent(req.TargetId);
                    res.ItemResults = await Reward.GetRewards(userDb, req.UserId, friendEvent.RewardId, "AddFriendEventRewardInvite");
                    return true;
                });
            }
            
            res.ResultType = AkaEnum.ResultType.Success;
            return res;
        }

        private async Task<bool> IsAlreadyReceivedReward(DBContext userDb, uint userId, uint rewardNum)
        {
            _query.Clear();
            _query.Append("SELECT userId FROM event_received_reward WHERE userId = ")
                .Append(userId).Append(" AND rewardNum = ").Append(rewardNum).Append(";");

            using (var cursor = await userDb.ExecuteReaderAsync(_query.ToString()))
            {
                return cursor.Read();
            }
        }

        private async Task<bool> IsValidRewardNum(DBContext userDb, uint userId, uint rewardNum)
        {
            _query.Clear();
            _query.Append("SELECT COUNT(userId) FROM event_friends WHERE userId = ")
                .Append(userId).Append(";");
            
            using (var cursor = await userDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return false;

                return cursor.GetInt32(0) >= rewardNum;
            }            
        }

        private async Task AddReward(DBContext userDb, uint userId, uint rewardNum)
        {
            _query.Clear();
            _query.Append("INSERT INTO event_received_reward (userId, rewardNum) VALUES (")
                .Append(userId).Append(",").Append(rewardNum).Append(");");

            await userDb.ExecuteNonQueryAsync(_query.ToString());
        }
    }
}
