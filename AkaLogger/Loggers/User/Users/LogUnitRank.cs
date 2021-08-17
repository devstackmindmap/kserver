using System;

namespace AkaLogger.Users
{
    public sealed class LogUnitRank
    {
        public void Log(uint userId, uint unitId, uint rankLevel, int rankPoint)
        {
            Logger.Instance().Analytics("UnitRank", "UnitRank",
                "UserId", userId.ToString(),
                "UnitId", unitId.ToString(),
                "RankLevel", rankLevel.ToString(),
                "RankPoint", rankPoint.ToString());
        }


        public void Log(uint userId, uint unitId, uint rankLevel)
        {
            Logger.Instance().Analytics("UnitRankUp", "UnitRankUp",
                "UserId", userId.ToString(),
                "UnitId", unitId.ToString(),
                "RankLevel", rankLevel.ToString());
        }
    }
}
