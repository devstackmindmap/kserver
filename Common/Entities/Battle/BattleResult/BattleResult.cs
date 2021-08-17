using AkaEnum;
using CommonProtocol;
using System.Threading.Tasks;

namespace Common.Entities.Battle
{
    public abstract class BattleResult : IBattleResultManager
    {
        public abstract string GetCountryCode();
        protected abstract Task Win();
        protected abstract Task Lose();
        protected abstract Task Draw();

        public abstract Task<uint> SeasonJob();

        public abstract Task<bool> RedisJob(uint serverCurrentSeason, uint clanId, string clanCountryCode);

        public abstract bool HasRedisJob();

        public virtual async Task<int> GetSumOfUnitsRankPoint()
        {
            return 0;
        }

        public abstract Task<ProtoNewInfusionBox> InfusionBoxJob(BattleResultType battleResultType);

        public async Task BattleResultJob(BattleResultType battleResultType)
        {
            if (battleResultType == BattleResultType.Win)
                await Win();
            else if (battleResultType == BattleResultType.Lose)
                await Lose();
            else
                await Draw();
        }


    }
}
