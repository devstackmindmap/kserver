using System;

namespace AkaLogger.Users
{
    public sealed class LogLogin
    {
        public void Log(uint userId)
        {
            Logger.Instance().Analytics("Login", "Login",
                "UserId", userId.ToString());
        }
    }
}
