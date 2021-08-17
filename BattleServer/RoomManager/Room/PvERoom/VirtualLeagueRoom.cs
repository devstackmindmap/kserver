using AkaEnum.Battle;
using AkaLogger;
using AkaSerializer;
using CommonProtocol;
using Network;
using System.Linq;
using System.Threading.Tasks;

namespace BattleServer
{
    public class VirtualLeagueRoom : PvERoom
    {
        public VirtualLeagueRoom(string roomId, IBattleInfo battleInfo)
            :base(roomId, battleInfo)
        {
           
        }

        protected override async Task<byte[]> GetBattleResultFromGameServerAsync(PlayerType winPlayer, uint stageLevelId)
        {
            _ = DeleteBattlePlayerInfoRedisKey();
            var playerInfoList = GetPlayerInfoList(winPlayer);

            WebServerRequestor webServer = new WebServerRequestor();
            return await webServer.RequestAsync(MessageType.GetBattleResultVirtualLeague, AkaSerializer<ProtoBattleResult>.Serialize(new ProtoBattleResult
            {
                PlayerInfoList = playerInfoList,
                BattleType = PlayerInfo.BattleType,
                MessageType = MessageType.GetBattleResultVirtualLeague,
            }), GetWebServerIp());
        }

        protected override void SendBattleResultToClient(byte[] battleResult, string session, PlayerType winPlayer)
        {
            TrySend(session, MessageType.GetBattleResultVirtualLeague, battleResult);
            LogBattleEndResult(battleResult, winPlayer);
        }

        private void LogBattleEndResult(byte[] battleResult, PlayerType winPlayer)
        {
            var onBattleResult = AkaSerializer<ProtoOnBattleResultRankData>.Deserialize(battleResult);

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
                onBattleResult.UserRankData?.MaxRankLevel ?? 0);
        }
    }
}
