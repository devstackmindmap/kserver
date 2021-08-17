using AkaData;
using AkaEnum.Battle;
using AkaRedisLogic;
using AkaSerializer;
using AkaUtility;
using Common;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BattleServer.Controller.Controllers
{
    public abstract class EnterPvPRoom : EnterRoom
    {
        private static Object _lockEnterRoom = new Object();

        protected bool MakeRoom(BattleInfo playerInfo)
        {
            if (RoomManager.MakeRoom(_roomId, playerInfo))
            {
                var delayTime = Data.GetConstant(AkaEnum.DataConstantType.PVP_MATCHING_WAITING_TIME).Value + ConstValue.ENTER_ROOM_WAITING_SECOND;
                Utility.CallDelayAfter( (int)(delayTime * 1.2) * 1000, delegate  {

                    var playerInfoForRoom = RoomManager.GetPlayerInfo(_roomId);
                    if (playerInfoForRoom.Player2Ready == false)
                    {
                        var player1Session = MainServer.Instance.GetSessionByID(playerInfo.SessionId);
                        SendEnterRoomFail(player1Session, AkaEnum.ResultType.Fail);
                        RoomManager.RemoveRoom(_roomId);
                    }
                });
                return true;
            }
            return false;
        }

        protected bool IsRoomFullAndTryEnterRoom(BattleInfo playerInfo)
        {
            var enterRoomFailTime = DateTime.UtcNow.AddSeconds(ConstValue.ENTER_ROOM_WAITING_SECOND);

            while (enterRoomFailTime > DateTime.UtcNow)
            {
                lock (_lockEnterRoom)
                {
                    if (RoomManager.TryEnterGuestPlayer(_roomId, playerInfo))
                        return true;
                }
            }
            return false;
        }

        protected override async Task SetBattlePlayingInfo(uint userId, string battleServerIp)
        {
            var member = KeyMaker.GetMemberKey(userId);
            var player1Member = RoomManager.GetPlayer1MemberKey(_roomId);
            var redis = AkaRedis.AkaRedis.GetDatabase();
            var port = MainServer.Instance.Config.Port;
            await BattleRedisJob.AddBattlePlayingInfoAsync(redis, _roomId, battleServerIp, port.ToString(), member, player1Member);
        }

        protected override void SendBeforeBattleStart(Dictionary<PlayerType, ProtoBeforeBattleStart> protoBattleStarts, NetworkSession session)
        {
            var player1SessionId = RoomManager.GetPlayer1SessionId(_roomId);

            MainServer.Instance.GetSessionByID(player1SessionId).Send(MessageType.BeforeBattleStart,
                AkaSerializer<ProtoBeforeBattleStart>.Serialize(protoBattleStarts[PlayerType.Player1]));
            session.Send(MessageType.BeforeBattleStart,
                AkaSerializer<ProtoBeforeBattleStart>.Serialize(protoBattleStarts[PlayerType.Player2]));
        }
    }
}
