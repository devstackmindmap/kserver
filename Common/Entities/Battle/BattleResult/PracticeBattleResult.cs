using AkaEnum;
using CommonProtocol;
using System.Threading.Tasks;

namespace Common.Entities.Battle
{
    public class PracticeBattleResult : BattleResult
    {
        private ProtoOnBattleResult _protoOnBattleResult;

        public PracticeBattleResult(ProtoOnBattleResult protoOnBattleResult)
        {
            _protoOnBattleResult = protoOnBattleResult;
        }

        protected override async Task Win()
        {
            _protoOnBattleResult.BattleResultType = BattleResultType.Win;
        }

        protected override async Task Lose()
        {
            _protoOnBattleResult.BattleResultType = BattleResultType.Lose;
        }

        protected override async Task Draw()
        {
            _protoOnBattleResult.BattleResultType = BattleResultType.Draw;
        }

        public override bool HasRedisJob()
        {
            return false;
        }

        public override async Task<bool> RedisJob(uint serverCurrentSeason, uint clanId, string clanCountryCode)
        {
            //Nothing
            return true;
        }

        public override async Task<uint> SeasonJob()
        {
            return 0;
        }

        public override string GetCountryCode()
        {
            return "";
        }

        public override async Task<ProtoNewInfusionBox> InfusionBoxJob(BattleResultType battleResultType)
        {
            return null;
        }
    }
}
