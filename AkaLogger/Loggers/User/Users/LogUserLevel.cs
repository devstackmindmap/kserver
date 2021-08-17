using System;

namespace AkaLogger.Users
{
    public sealed class LogUserLevel
    {
        public void Log(uint userId, uint userLevel, ulong userExp)
        {
            Logger.Instance().Analytics("UserLevel", "UserLevel",
                "UserId", userId.ToString(),
                "UserLevel", userLevel.ToString(),
                "UserExp", userExp.ToString());
        }
    }
}
