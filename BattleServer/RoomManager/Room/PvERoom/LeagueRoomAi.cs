using AkaEnum.Battle;
using AkaLogger;
using AkaSerializer;
using BattleLogic;
using CommonProtocol;
using Network;
using System.Linq;
using System.Threading.Tasks;

namespace BattleServer
{
    public class LeagueRoomAi : PvERoom
    {
        public LeagueRoomAi(string roomId, IBattleInfo battleInfo)
            : base(roomId, battleInfo)
        {
        }

        protected override async Task<byte[]> GetBattleResultFromGameServerAsync(PlayerType winPlayer, uint stageLevelId)
        {
            _ = DeleteBattlePlayerInfoRedisKey();
            var playerInfoList = GetPlayerInfoList(winPlayer);

            WebServerRequestor webServer = new WebServerRequestor();
            return await webServer.RequestAsync(MessageType.GetBattleResultKnightLeague, AkaSerializer<ProtoBattleResult>.Serialize(new ProtoBattleResult
            {
                PlayerInfoList = playerInfoList,
                BattleType = PlayerInfo.BattleType,
                MessageType = MessageType.GetBattleResultKnightLeague,
            }), GetWebServerIp());
        }

        protected override void SendBattleResultToClient(byte[] battleResult, string session, PlayerType winPlayer)
        {
            var onBattleResult = AkaSerializer<ProtoOnBattleResultRankList>.Deserialize(battleResult);

            if (onBattleResult.BattleResult.TryGetValue(PlayerInfo.Player1UserId, out var playerBattleResult))
            {
                var resBytes = AkaSerializer<ProtoOnBattleResultRank>.Serialize(playerBattleResult);
                TrySend(session, MessageType.GetBattleResultKnightLeague, resBytes);
            }

            LogBattleEndResult(onBattleResult, winPlayer);
        }

        private void LogBattleEndResult(ProtoOnBattleResultRankList onBattleResultRank, PlayerType winPlayer)
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
                onBattleResultRank.BattleResult[PlayerInfo.Player1UserId].UserRankData?.MaxRankLevel ?? 0);
        }
    }
}
