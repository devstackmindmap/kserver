namespace AkaLogger.Users
{
    public sealed class LogJoin
    {
        public void Log(uint userId, string nickName, string pushKey, byte pushAgree, byte nightPushAgree, byte termsAgree)
        {
            Logger.Instance().Analytics("Join", "Join",
                "UserId", userId.ToString(),
                "NickName", nickName,
                "PushKey", pushKey,
                "PushAgree", pushAgree.ToString(),
                "NightPushAgree", nightPushAgree.ToString(),
                "TermsAgree", termsAgree.ToString());
        }
    }
}
