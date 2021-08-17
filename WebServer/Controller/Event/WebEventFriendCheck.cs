using AkaDB.MySql;
using Common.Entities.Reward;
using CommonProtocol;
using System;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Controller.Event
{
    public class WebEventFriendCheck : BaseController
    {
        private StringBuilder _query = new StringBuilder();

        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserId;
            var res = new ProtoUintAndUintList();

            using (var userDb = new DBContext(req.UserId))
            {
                res.Num = await GetEventFriendNum(userDb, req.UserId);
                
                await SetReceivedReward(userDb, req.UserId, res);
            }
            return res;
        }

        private async Task SetReceivedReward(DBContext userDb, uint userId, ProtoUintAndUintList res)
        {
            _query.Clear();
            _query.Append("SELECT rewardNum FROM event_received_reward WHERE userId = ")
                .Append(userId).Append(";");

            using (var cursor = await userDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    res.Items.Add((uint)cursor["rewardNum"]);
                }
            }
        }

        private async Task<uint> GetEventFriendNum(DBContext userDb, uint userId)
        {
            _query.Clear();
            _query.Append("SELECT COUNT(userId) FROM event_friends WHERE userId = ")
                .Append(userId).Append(";");
            
            using (var cursor = await userDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return 0;

                return (uint)cursor.GetInt32(0);
            }            
        }
    }
}
