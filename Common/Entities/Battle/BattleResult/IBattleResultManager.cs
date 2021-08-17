using AkaEnum;
using CommonProtocol;
using System.Threading.Tasks;

namespace Common.Entities.Battle
{
    public interface IBattleResultManager
    {
        string GetCountryCode();

        Task<uint> SeasonJob();

        Task BattleResultJob(BattleResultType battleResultType);

        Task<bool> RedisJob(uint serverCurrentSeason, uint clanId, string clanCountryCode);

        Task<ProtoNewInfusionBox> InfusionBoxJob(BattleResultType battleResultType);

        bool HasRedisJob();

        Task<int> GetSumOfUnitsRankPoint();

    }
}
