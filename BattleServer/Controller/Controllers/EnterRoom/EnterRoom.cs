using AkaData;
using AkaEnum;
using AkaEnum.Battle;
using AkaSerializer;
using AkaUtility;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BattleServer.Controller.Controllers
{
    public abstract class EnterRoom : BaseController
    {
        protected string _roomId;
        protected abstract void SendBeforeBattleStart(Dictionary<PlayerType, ProtoBeforeBattleStart> protoBattleStarts, NetworkSession session);
        protected abstract Task SetBattlePlayingInfo(uint userId, string battleServerIp);

        protected async Task<(Dictionary<PlayerType, ProtoBeforeBattleStart> protoBattleStarts, ResultType resultType)> BattleReady()
        {
            var resultType = await RoomManager.BattleInitialize(_roomId);
            if (resultType != ResultType.Success)
                return (null, resultType);
            

            var protoBattleStarts = new Dictionary<PlayerType, ProtoBeforeBattleStart>(PlayerTypeComparer.Comparer);
            protoBattleStarts.Add(PlayerType.Player1, new ProtoBeforeBattleStart
            {
                RoomId = _roomId
            });
            protoBattleStarts.Add(PlayerType.Player2, new ProtoBeforeBattleStart
            {
                RoomId = _roomId
            });

            // Data
            RoomManager.FillBattleStartInfo(_roomId, protoBattleStarts);

            return (protoBattleStarts, ResultType.Success);
        }

        protected async Task DelayBattleStart(bool isStart = true)
        {
            DateTime battleStartTime;
            if (isStart)
                battleStartTime = DateTime.UtcNow.AddSeconds(Data.GetConstant(AkaEnum.DataConstantType.PVP_MATCHING_WAITING_TIME).Value);
            else
                battleStartTime = DateTime.UtcNow.AddSeconds(Data.GetConstant(AkaEnum.DataConstantType.BETWEEN_ROUND_WAITING_TIME).Value);

            var ticks = battleStartTime.Ticks - DateTime.UtcNow.Ticks;
            if (ticks > 0)
                await Task.Delay(TimeSpan.FromTicks(ticks));

            RoomManager.BattleStart(_roomId);
        }

        protected virtual async Task<ResultType> EnterRoomSuccess(BattleInfo playerInfo, NetworkSession session, bool isStart = true)
        {
            try
            {
                _ = SetBattlePlayingInfo(playerInfo.UserId, playerInfo.BattleServerIp);
                var battleReadyResult = await BattleReady();
                if (battleReadyResult.resultType != ResultType.Success)
                    return battleReadyResult.resultType;

                var protoBattleStarts = battleReadyResult.protoBattleStarts;

                SendBeforeBattleStart(protoBattleStarts, session);
                _ = DelayBattleStart();
            }
            catch(Exception e)
            {
                await RoomManager.RemoveRoomForException(_roomId);
                AkaLogger.Log.Debug.Exception("EnterRoomSuccess", e);
                return ResultType.Fail;
            }

            return ResultType.Success;
        }

        protected void SendEnterRoomFail(NetworkSession session, ResultType resultType /* , reason */)
        {
            //TODO add reason
            session?.Send(MessageType.EnterRoomFail, AkaSerializer<ProtoResult>.Serialize(new ProtoResult
            {
                ResultType = resultType
            })); ;
        }
    }
}
