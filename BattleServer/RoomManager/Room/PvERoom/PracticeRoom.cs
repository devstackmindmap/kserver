using AkaEnum.Battle;
using AkaSerializer;
using CommonProtocol;
using System.Threading.Tasks;

namespace BattleServer
{
    public class PracticeRoom : PvERoom
    {
        public PracticeRoom(string roomId, IBattleInfo battleInfo)
            :base(roomId, battleInfo)
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
