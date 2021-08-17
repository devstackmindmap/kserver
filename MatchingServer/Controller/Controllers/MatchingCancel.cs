using AkaRedisLogic;
using AkaSerializer;
using Common;
using CommonProtocol;
using System;
using System.Threading.Tasks;

namespace MatchingServer
{
    public class MatchingCancel : BaseController
    {
        public override async Task DoPipeline(NetworkSession session, BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoMatchingCancel;
            var redis = AkaRedis.AkaRedis.GetDatabase();
            var member = KeyMaker.GetMemberKey(req.UserId);

            var messageType = MessageType.MatchingCancelFail;

            //AkaLogger.Log.Matching.TryCancel.Log(member);

            var waiting = DateTime.UtcNow.AddMilliseconds(ConstValue.MATCHING_CANCEL_WAITING_MILLISECOND);
            for (int i = 0; i < 2; i++)
            {
                using( var lockEnter = await LockObject.LockEnterRoomAsync())
                {
                    var roomInfoRedisValue = await MatchingRedisJob.GetRoomInfoAsync(redis, member);
                    if (roomInfoRedisValue.HasValue)
                    {
                        var roomInfo = AkaSerializer<RedisValueRoomInfo>.Deserialize(roomInfoRedisValue);
                        await MatchingCommonJob.CleanRoomInfo(redis, roomInfo.MatchingGroupKey, member, roomInfo.RoomId, roomInfo.SessionId);
                        messageType = MessageType.MatchingCancelSuccess;

                        //AkaLogger.Log.Matching.Cancel.Log(roomInfo.RoomId, roomInfo.Member, roomInfo.MatchingGroupKey, roomInfo.UserRankPoint);
                        break;
                    }
                }
                while (waiting > DateTime.UtcNow) ;
            }

            //if (messageType == MessageType.MatchingCancelFail)
            //{
            //    AkaLogger.Log.Matching.Cancel.Log(member);
            //}
            session.Send(messageType, AkaSerializer<ProtoEmpty>.Serialize(new ProtoEmpty
            {
                MessageType = messageType
            }));
        }
    }
}
