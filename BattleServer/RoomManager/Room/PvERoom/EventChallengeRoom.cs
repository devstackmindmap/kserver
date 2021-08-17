using AkaData;
using AkaEnum.Battle;
using AkaLogger;
using AkaSerializer;
using BattleLogic;
using CommonProtocol;
using Network;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleServer
{
    public class EventChallengeRoom : PvERoom
    {
        private new BattleInfoEventChallenge _battleInfo;

        public EventChallengeRoom(string roomId, IBattleInfo battleInfo)
            : base(roomId, battleInfo)
        {
            _battleInfo = battleInfo as BattleInfoEventChallenge;
        }

        protected override async Task<byte[]> GetBattleResultFromGameServerAsync(PlayerType winPlayer, uint stageLevelId)
        {
            var battleResultType = GetBattleResultType(winPlayer, PlayerType.Player1);

            WebServerRequestor webServer = new WebServerRequestor();

            if (battleResultType == AkaEnum.BattleResultType.Win && Data.IsValidStageRound(_battleInfo.StageLevelId, _battleInfo.Round + 1))
            {
                await webServer.RequestAsync(MessageType.BattleRoundClearEventChallenge,
                    AkaSerializer<ProtoEventChallenge>.Serialize(new ProtoEventChallenge
                    {
                        MessageType = MessageType.BattleRoundClearEventChallenge,
                        ChallengeEventId = _battleInfo.ChallengeEventId,
                        DifficultLevel = _battleInfo.DifficultLevel,
                        IsStart = false,
                        UserId = _battleInfo.UserId,
                    }), GetWebServerIp());

                TrySend(SessionIdByPlayer(PlayerType.Player1), MessageType.BattleEventChallengeRoundResult, AkaSerializer<ProtoEmpty>
                    .Serialize(new ProtoEmpty()));

                return null;
            }

            _ = DeleteBattlePlayerInfoRedisKey();
            var playerInfoList = GetPlayerInfoList(winPlayer);

            return await webServer.RequestAsync(MessageType.GetBattleResultEventChallenge,
                AkaSerializer<ProtoBattleResultEventChallenge>.Serialize(new ProtoBattleResultEventChallenge
                {
                    PlayerInfoList = playerInfoList,
                    BattleType = PlayerInfo.BattleType,
                    MessageType = MessageType.GetBattleResultEventChallenge,
                    ChallengeEventId = _battleInfo.ChallengeEventId,
                    DifficultLevel = _battleInfo.DifficultLevel
                }), GetWebServerIp());

        }

        protected override void SendBattleResultToClient(byte[] battleResult, string session, PlayerType winPlayer)
        {
            TrySend(session, MessageType.GetBattleResultChallenge, battleResult);
            var onBattleResult = AkaSerializer<ProtoOnBattleResult>.Deserialize(battleResult);
            LogBattleEndResult(onBattleResult, winPlayer);
        }

        private void LogBattleEndResult(ProtoOnBattleResult onBattleResultRank, PlayerType winPlayer)
        {
            var player1UnitsInfo = GetAliveAndDeathUnits(Battle.Players[PlayerType.Player1]);
            var player2UnitsInfo = GetAliveAndDeathUnits(Battle.Players[PlayerType.Player2]);
            var player1UnitLevels = player1UnitsInfo.Values;
            var player1Cards = Battle.BattleHelper.BattleController.GetBattleCards()[PlayerType.Player1].AllCards.Select(card => card.CardId);

            Log.Battle.BattleEndResult.Log(RoomId, (byte)PlayerInfo.BattleType, winPlayer.ToString(),
                false,
                PlayerInfo.Player1UserId, PlayerInfo.Player2UserId,
                Battle.Players[PlayerType.Player1].PlayerIdentifier.Nickname, Battle.Players[PlayerType.Player2].PlayerIdentifier.Nickname,
                Battle.Status.StartDateTime, Battle.Status.EndDateTime,
                player1UnitsInfo.Keys,
                player2UnitsInfo.Keys,
                player1UnitLevels, player1Cards,
                0);
        }

        protected override AdditionalBattleInfo GetAdditionalBattleInfo()
        {
            return new AdditionalBattleInfo
            {
                BanCardIdList = GetBanCardIdList()
            };
        }

        private List<uint> GetBanCardIdList()
        {
            var battleInfo = _battleInfo as BattleInfoEventChallenge;
            return Data.GetDataChallengeEvent(battleInfo.ChallengeEventId).BanCardIdList;
        }
    }
}
