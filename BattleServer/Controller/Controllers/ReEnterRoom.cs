using AkaRedisLogic;
using AkaSerializer;
using CommonProtocol;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace BattleServer.Controller.Controllers
{
    class ReEnterRoom : BaseController
    {

        public override async Task DoPipeline(NetworkSession session, BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoReEnterRoom;
            AkaLogger.Log.Battle.ReEnterRoom.LogTryReEnterRoom(req.UserId);

            var member = KeyMaker.GetMemberKey(req.UserId);
            var redis = AkaRedis.AkaRedis.GetDatabase();
            var battlePlayingInfo = await GameBattleRedisJob.GetBattlePlayingInfoAsync(redis, member);

            if (battlePlayingInfo == null)
            {
                NotBattlePlaying(redis, member, session, RoomStatus.Waiting);
                //AkaLogger.Log.Battle.ReEnterRoom.Log(req.UserId,"NotBattlePlaying");
                return;
            }

            if (!IsOnBattle(new DateTime(battlePlayingInfo.BattleStartDate)))
            {
                NotBattlePlaying(redis, member, session, RoomStatus.Waiting);
                //AkaLogger.Log.Battle.ReEnterRoom.Log(req.UserId, "InvalidBattleData");
                return;
            }

            var roomStatus = RoomManager.GetRoomStatus(battlePlayingInfo.RoomId);
            if (RoomStatus.InBattle != roomStatus)
            {
                NotBattlePlaying(redis, member, session, roomStatus);
                //AkaLogger.Log.Battle.ReEnterRoom.Log(req.UserId, "DontHaveRoom");
                return;
            }

            ProtoCurrentBattleStatus protoCurrentBattleStatus = new ProtoCurrentBattleStatus();
            RoomManager.ReEnterRoom(battlePlayingInfo.RoomId, req.UserId, session.SessionID, protoCurrentBattleStatus);

            //AkaLogger.Log.Battle.ReEnterRoom.Log(req.UserId, (byte)protoCurrentBattleStatus.BattleType, battlePlayingInfo.RoomId);
            session.Send(MessageType.ReEnterRoomSuccess, AkaSerializer<ProtoCurrentBattleStatus>.Serialize(protoCurrentBattleStatus));
        }

        private bool IsOnBattle(DateTime battleStartTime)
        {
            if (battleStartTime.AddMinutes(10) <= DateTime.UtcNow)
                return false;

            return true;
        }

        
        private void NotBattlePlaying(IDatabase redis, string member, NetworkSession session, RoomStatus roomStatus)
        {
            var failedMessage = MessageType.ReEnterRoomFailAtNotyet;
            if (roomStatus != RoomStatus.BattleRoomMatched)
            {
                _ = GameBattleRedisJob.DeleteBattlePlayingInfoAsync(redis, member);
                failedMessage = MessageType.ReEnterRoomFail;
            }
            SendFail(session, failedMessage);
        }

        private void SendFail(NetworkSession session, MessageType message)
        {
            session.Send(message, AkaSerializer<ProtoEmpty>.Serialize(new ProtoEmpty
            {
                MessageType = message
            }));


        }
    }
}
