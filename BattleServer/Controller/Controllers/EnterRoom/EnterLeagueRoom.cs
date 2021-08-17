using CommonProtocol;
using System;
using System.Threading.Tasks;

namespace BattleServer.Controller.Controllers
{
    public class EnterLeagueRoom : EnterPvPRoom
    {
        public override async Task DoPipeline(NetworkSession session, BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoEnterRoom;
            session.UserId = req.UserId;

            var battleInfo = new BattleInfo
            {
                BattleType = req.BattleType,
                UserId = req.UserId,
                DeckNum = req.DeckNum,
                BattleServerIp = req.BattleServerIp,
                SessionId = session.SessionID
            };

            _roomId = req.RoomId;
            AkaLogger.Log.Battle.EnterRoom.LogForEnterRoom(battleInfo.UserId, (byte)battleInfo.BattleType, _roomId);

            if (MakeRoom(battleInfo))
                return;

            if (false == IsRoomFullAndTryEnterRoom(battleInfo))
            {
                SendEnterRoomFail(session, AkaEnum.ResultType.Fail);
                return;
            }

            var resultType = await EnterRoomSuccess(battleInfo, session);
            if (resultType != AkaEnum.ResultType.Success)
            {
                SendEnterRoomFail(session, resultType);
                return;
            }
        }
    }
}
