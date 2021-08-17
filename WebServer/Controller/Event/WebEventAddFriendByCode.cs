using AkaDB.MySql;
using Common.Entities.Reward;
using CommonProtocol;
using System;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Controller.Event
{
    public class WebEventAddFriendByCode : BaseController
    {
        private StringBuilder _query = new StringBuilder();

        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserIdTargetId;
            var res = new ProtoResult();

            // 이미 이벤트 친구라면 
            if (await IsEventFriend(req.UserId, req.TargetId))
            {
                res.ResultType = AkaEnum.ResultType.AlreadEventFriend;
                return res;
            }

            // 이미 이벤트 친구라면 
            if (await IsEventFriend(req.TargetId, req.UserId))
            {
                res.ResultType = AkaEnum.ResultType.AlreadEventFriend;
                return res;
            }

            // 이미 초대된 보상 받은적 있는지 확인 (계정당 한번만 해당 보상 받기 가능)
            if (await IsAlreadyReceivedReward(req.UserId))
            {
                res.ResultType = AkaEnum.ResultType.AlreadyRewarded;
                return res;
            }

            // 가입시간체크, 양쪽다 보상 없음
            if (false == await IsValidJoinDateTime(req.UserId))
            {
                res.ResultType = AkaEnum.ResultType.InvalidDateTime;
                return res;
            }

            // 친구체크, 양쪽다 보상 없음
            if (false == await IsFriend(req.UserId, req.TargetId))
            {
                res.ResultType = AkaEnum.ResultType.Fail;
                return res;
            }

            using (DBContext userDb = new DBContext(req.UserId), 
                targetDb = new DBContext(req.TargetId))
            {
                await targetDb.BeginTransactionCallback(async () =>
                {
                    // 초대한 친구의 이벤트 제한 확인
                    if (await IsValidTargetEventFriendCount(userDb, req.TargetId))
                    {
                        await AddEventFriend(userDb, req.TargetId, req.UserId);
                    }

                    await userDb.BeginTransactionCallback(async () =>
                    {
                        // 초대된 유저(userId) 보상 
                        await Reward.GetRewards(userDb, req.UserId, 161101, "AddFriendEventRewardInvited");
                        await SetEventReceivedReward2(userDb, req.UserId);
                        return true;
                    });
                    
                    return true;
                });
            }

            res.ResultType = AkaEnum.ResultType.Success;
            return res;
        }

        private async Task<bool> IsAlreadyReceivedReward(uint userId)
        {
            _query.Clear();
            _query.Append("SELECT userId FROM event_received_reward2 WHERE userId = ").Append(userId).Append(";");

            using (var userDb = new DBContext(userId))
            {
                using (var cursor = await userDb.ExecuteReaderAsync(_query.ToString()))
                {
                    return cursor.Read();
                }
            }
        }

        private async Task SetEventReceivedReward2(DBContext userDb, uint userId)
        {
            _query.Clear();
            _query.Append("INSERT INTO event_received_reward2 (userId) VALUES (").Append(userId).Append(");");
            await userDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private async Task<bool> IsValidJoinDateTime(uint userId)
        {
            _query.Clear();
            _query.Append("SELECT joinDateTime FROM accounts WHERE userId = ").Append(userId).Append(";");
            
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var cursor = await accountDb.ExecuteReaderAsync(_query.ToString()))
                {
                    if (false == cursor.Read())
                        return false;

                    var joinDateTime = (DateTime)cursor["joinDateTime"];

                    if ((DateTime.UtcNow - joinDateTime).TotalMinutes < 30)
                        return true;
                }
            }

            return false;
        }

        private async Task<bool> IsValidTargetEventFriendCount(DBContext userDb, uint userId)
        {
            _query.Clear();
            _query.Append("SELECT COUNT(userId) FROM event_friends WHERE userId = ").Append(userId).Append(" FOR UPDATE;");
            
            using (var cursor = await userDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return false;

                if (cursor.GetInt32(0) >= 10)
                    return false;
            }

            return true;
        }

        private async Task AddEventFriend(DBContext userDb, uint userId, uint targetId)
        {
            _query.Clear();
            _query.Append("INSERT INTO event_friends (userId, friendId) VALUES (").Append(userId)
                .Append(",").Append(targetId).Append(");");

            await userDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private async Task<bool> IsFriend(uint userId, uint targetId)
        {
            _query.Clear();
            _query.Append("SELECT userId FROM friends WHERE userId = ").Append(userId)
                .Append(" AND friendId = ").Append(targetId).Append(";");

            using (var accountDb = new DBContext(userId))
            {
                using (var cursor = await accountDb.ExecuteReaderAsync(_query.ToString()))
                {
                    return cursor.Read();
                }
            }
        }

        private async Task<bool> IsEventFriend(uint userId, uint targetId)
        {
            _query.Clear();
            _query.Append("SELECT userId FROM event_friends WHERE userId = ").Append(userId)
                .Append(" AND friendId = ").Append(targetId).Append(";");

            using (var accountDb = new DBContext(userId))
            {
                using (var cursor = await accountDb.ExecuteReaderAsync(_query.ToString()))
                {
                    return cursor.Read();
                }
            }
        }
    }
}
