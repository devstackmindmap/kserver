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
    public class ChallengeRoom : PvERoom
    {
        private new BattleInfoChallenge _battleInfo;

        public ChallengeRoom(string roomId, IBattleInfo battleInfo)
            : base(roomId, battleInfo)
        {
            _battleInfo = battleInfo as BattleInfoChallenge;
        }

        protected override async Task<byte[]> GetBattleResultFromGameServerAsync(PlayerType winPlayer, uint stageLevelId)
        {
            var battleResultType = GetBattleResultType(winPlayer, PlayerType.Player1);

            WebServerRequestor webServer = new WebServerRequestor();

            if (battleResultType == AkaEnum.BattleResultType.Win && Data.IsValidStageRound(_battleInfo.StageLevelId, _battleInfo.Round + 1))
            {
                await webServer.RequestAsync(MessageType.BattleRoundClearChallenge, AkaSerializer<ProtoChallenge>.Serialize(new ProtoChallenge
                {
                    MessageType = MessageType.BattleRoundClearChallenge,
                    Season = _battleInfo.Season,
                    Day = _battleInfo.Day,
                    DifficultLevel = _battleInfo.DifficultLevel,
                    IsStart = false,
                    UserId = _battleInfo.UserId,
                }), GetWebServerIp());

                TrySend(SessionIdByPlayer(PlayerType.Player1), MessageType.BattleChallengeRoundResult, AkaSerializer<ProtoEmpty>
                    .Serialize(new ProtoEmpty()));

                return null;
            }

            _ = DeleteBattlePlayerInfoRedisKey();
            var playerInfoList = GetPlayerInfoList(winPlayer);

            return await webServer.RequestAsync(MessageType.GetBattleResultChallenge, AkaSerializer<ProtoBattleResultChallenge>.Serialize(new ProtoBattleResultChallenge
            {
                PlayerInfoList = playerInfoList,
                BattleType = PlayerInfo.BattleType,
                MessageType = MessageType.GetBattleResultChallenge,
                Season = _battleInfo.Season,
                Day = _battleInfo.Day,
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
                BanCardIdList = GetBanCardIdList(),
                AffixList = GetAffixList()
            };
        }

        private List<uint> GetBanCardIdList()
        {
            var battleInfo = _battleInfo as BattleInfoChallenge;
            return Data.GetDataChallenge(battleInfo.Season, battleInfo.Day).BanCardIdList;
        }
        
        private List<uint> GetAffixList()
        {
            if (_battleInfo.DifficultLevel == 2)
                return Data.GetDataChallengeAffix((uint)_battleInfo.Season).NormalAffixIdList;
            else if (_battleInfo.DifficultLevel == 3)
                return Data.GetDataChallengeAffix((uint)_battleInfo.Season).HardAffixIdList;
            return null;
        }
    }
}
