using AkaData;
using AkaEnum.Battle;
using AkaLogger;
using AkaRedisLogic;
using AkaSerializer;
using CommonProtocol;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BattleServer.Controller.Controllers
{
    public abstract class EnterCommonPvERoom : EnterRoom
    {
        public async Task DoProcess(NetworkSession session, BattleInfo battleInfo, bool isStart = true)
        {
            Log.Battle.EnterRoom.LogForEnterRoom(battleInfo.UserId, (byte)battleInfo.BattleType, _roomId);

            var resultType = await PveInfoFactory.SetBattleInfo(battleInfo);
            if (resultType != AkaEnum.ResultType.Success)
            {
                SendEnterRoomFail(session, resultType);
                Logger.Instance().Info($"{GetType()} InvalidStageRound info : UserId[{battleInfo.UserId.ToString()}]");
                return;
            }

            Logger.Instance().Info($"EnterPveRoom:UserId[{battleInfo.UserId.ToString()}]");

            if (false == MakeRoom(battleInfo))
            {
                SendEnterRoomFail(session, AkaEnum.ResultType.Fail);
                return;
            }

            if (false == EnemyTryEnterRoom(battleInfo))
            {
                SendEnterRoomFail(session, AkaEnum.ResultType.Fail);
                return;
            }

            resultType = await EnterRoomSuccess(battleInfo, session, isStart);
            if (AkaEnum.ResultType.Success != resultType)
            {
                SendEnterRoomFail(session, resultType);
                return;
            }
        }

        public async Task DoNextRound(BattleInfo playerInfo)
        {
            var session = MainServer.Instance.GetSessionByID(playerInfo.SessionId);
            await DoProcess(session, playerInfo);
        }

        private bool MakeRoom(BattleInfo battleInfo)
        {
            _roomId = KeyMaker.GetNewRoomId();
            return RoomManager.MakeRoom(_roomId, battleInfo);
        }

        private bool EnemyTryEnterRoom(BattleInfo battleInfo)
        {
            return RoomManager.TryEnterGuestPlayer(_roomId, battleInfo);
        }

        protected override async Task SetBattlePlayingInfo(uint userId, string battleServerIp)
        {
            var member = KeyMaker.GetMemberKey(userId);
            var redis = AkaRedis.AkaRedis.GetDatabase();
            var port = MainServer.Instance.Config.Port;
            await BattleRedisJob.AddBattlePlayingInfoAsync(redis, _roomId, battleServerIp, port.ToString(), member);
        }

        protected override void SendBeforeBattleStart(Dictionary<PlayerType, ProtoBeforeBattleStart> protoBattleStarts, NetworkSession session)
        {
            session.Send(MessageType.BeforeBattleStart,
                AkaSerializer<ProtoBeforeBattleStart>.Serialize(protoBattleStarts[PlayerType.Player1]));
        }

        protected override async Task<AkaEnum.ResultType> EnterRoomSuccess(BattleInfo playerInfo, NetworkSession session, bool isStart)
        {
            try
            {
                _ = SetBattlePlayingInfo(playerInfo.UserId, playerInfo.BattleServerIp);
                var battleReadyResult = await BattleReady();

                if (battleReadyResult.resultType != AkaEnum.ResultType.Success)
                    return battleReadyResult.resultType;

                var protoBattleStarts = battleReadyResult.protoBattleStarts;

                SendBeforeBattleStart(protoBattleStarts, session);

                if (!HasStartEvent(playerInfo))
                    _ = DelayBattleStart(isStart);
            }
            catch (Exception e)
            {
                await RoomManager.RemoveRoomForException(_roomId);
                AkaLogger.Log.Debug.Exception("EnterRoomSuccess", e);
                return AkaEnum.ResultType.Fail;
            }
            return AkaEnum.ResultType.Success;
        }

        protected bool HasStartEvent(BattleInfo playerInfo)
        {
            return !string.IsNullOrWhiteSpace(Data.GetStageRound(playerInfo.StageRoundId).StartDialogFileName);
        }
    }
}
