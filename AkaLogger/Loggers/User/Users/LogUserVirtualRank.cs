using System;

namespace AkaLogger.Users
{
    public sealed class LogUserVirtualRank
    {
        public void Log(uint userId, uint rankLevel, int rankPoint)
        {
            Logger.Instance().Analytics("UserVirtualRank", "UserRank",
                "UserId", userId.ToString(),
                "RankLevel", rankLevel.ToString(),
                "RankPoint", rankPoint.ToString());
        }
    }
}
