using AkaEnum.Battle;
using System.Collections.Generic;
using AkaUtility;
using System.Threading.Tasks;
using Network;
using CommonProtocol;
using AkaSerializer;

namespace BattleServer
{
    public class FriendlyRoom : PvPRoom
    {
        private readonly Dictionary<PlayerType, string> _sessionIds = new Dictionary<PlayerType, string>(PlayerTypeComparer.Comparer)
        {
            {PlayerType.Player1, string.Empty},
            {PlayerType.Player2, string.Empty}
        };

        public FriendlyRoom(string roomId, IBattleInfo playerInfo)
            : base(roomId, playerInfo)
        {
            
        }

        protected override async Task<byte[]> GetBattleResultFromGameServerAsync(PlayerType winPlayer, uint stageLevelId)
        {
            _ = DeleteBattlePlayerInfoRedisKey();
            return null;
        }

        protected override void SendBattleResultToClient(byte[] battleResult, string session, PlayerType winPlayer)
        {
        }
    }
}
