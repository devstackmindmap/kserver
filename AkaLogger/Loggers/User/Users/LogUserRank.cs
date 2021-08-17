using System;

namespace AkaLogger.Users
{
    public sealed class LogUserRank
    {
        public void Log(uint userId, uint rankLevel, int rankPoint)
        {
            Logger.Instance().Analytics("UserRank", "UserRank",
                "UserId", userId.ToString(),
                "RankLevel", rankLevel.ToString(),
                "RankPoint", rankPoint.ToString());
        }
    }
}
