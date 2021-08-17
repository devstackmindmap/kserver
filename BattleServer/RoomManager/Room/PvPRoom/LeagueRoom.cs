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
    public class LeagueRoom : PvPRoom
    {
        public LeagueRoom(string roomId, IBattleInfo battleInfo)
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

            if (onBattleResult.BattleResult.ContainsKey(PlayerInfo.Player1UserId))
                PlayerEnd(onBattleResult.BattleResult[PlayerInfo.Player1UserId], PlayerType.Player1);

            if (onBattleResult.BattleResult.ContainsKey(PlayerInfo.Player2UserId))
                PlayerEnd(onBattleResult.BattleResult[PlayerInfo.Player2UserId], PlayerType.Player2);

            LogBattleEndResult(onBattleResult, winPlayer);
        }

        protected void PlayerEnd<T>(T playerBattleResult, PlayerType who)
        {
            var resBytes = AkaSerializer<T>.Serialize(playerBattleResult);
            Battle.BattleHelper.BattleRecorder.BattleResultRecord(new BattleSendData
            {
                Data = resBytes,
                MessageType = MessageType.GetBattleResultKnightLeague,
                PlayerType = who
            });

            var session = SessionIdByPlayer(who);
            TrySend(session, MessageType.GetBattleResultKnightLeague, resBytes);
        }

        private void LogBattleEndResult(ProtoOnBattleResultRankList onBattleResultRank, PlayerType winPlayer)
        {
            var player1UnitsInfo = GetAliveAndDeathUnits(Battle.Players[PlayerType.Player1]);
            var player2UnitsInfo = GetAliveAndDeathUnits(Battle.Players[PlayerType.Player2]);
            var player1UnitLevels = player1UnitsInfo.Values;
            var player2UnitLevels = player2UnitsInfo.Values;
            var player1Cards = Battle.BattleHelper.BattleController.GetBattleCards()[PlayerType.Player1].AllCards.Select(card => card.CardId);
            var player2Cards = Battle.BattleHelper.BattleController.GetBattleCards()[PlayerType.Player2].AllCards.Select(card => card.CardId);

            Log.Battle.BattleEndResult.Log(RoomId, (byte)PlayerInfo.BattleType, winPlayer.ToString(),
                PlayerInfo.Player1UserId, PlayerInfo.Player2UserId,
                Battle.Players[PlayerType.Player1].PlayerIdentifier.Nickname,
                Battle.Players[PlayerType.Player2].PlayerIdentifier.Nickname,
                Battle.Status.StartDateTime, Battle.Status.EndDateTime,
                player1UnitsInfo.Keys,
                player2UnitsInfo.Keys,
                player1UnitLevels, player2UnitLevels,
                player1Cards, player2Cards,
                onBattleResultRank.BattleResult[PlayerInfo.Player1UserId].UserRankData?.MaxRankLevel ?? 0,
                onBattleResultRank.BattleResult[PlayerInfo.Player2UserId].UserRankData?.MaxRankLevel ?? 0);
        }
    }
}
