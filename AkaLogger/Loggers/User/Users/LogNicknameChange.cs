using System;

namespace AkaLogger.Users
{
    public sealed class LogNicknameChange
    {
        public void Log(uint userId, string newNickname)
        {
            Logger.Instance().Analytics("NicknameChange", "NicknameChange",
                "UserId", userId.ToString(),
                "NewNickname", newNickname);
        }
    }
}
