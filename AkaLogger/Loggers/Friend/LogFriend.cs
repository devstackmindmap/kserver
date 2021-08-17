namespace AkaLogger.Friend
{
    public sealed class LogFriend
    {
        public void FriendAddByCode(uint userId, string inviteCode)
        {
            Logger.Instance().Analytics("FriendAddByCode", "Friend",
                "UserId", userId.ToString(),
                "inviteCode", inviteCode);
        }

        public void FriendAddByRequested(uint userId, uint targetId)
        {
            Logger.Instance().Analytics("FriendAddByRequested", "Friend",
                "UserId", userId.ToString(),
                "TargetId", targetId.ToString());
        }

        public void FriendReject(uint userId, uint targetId)
        {
            Logger.Instance().Analytics("FriendReject", "Friend",
                "UserId", userId.ToString(),
                "TargetId", targetId.ToString());
        }

        public void FriendRemove(uint userId, uint targetId)
        {
            Logger.Instance().Analytics("FriendRemove", "Friend",
                "UserId", userId.ToString(),
                "TargetId", targetId.ToString());
        }

        public void FriendRequestById(uint userId, uint targetId)
        {
            Logger.Instance().Analytics("FriendRequestById", "Friend",
                "UserId", userId.ToString(),
                "TargetId", targetId.ToString());
        }

        public void FriendRequestByNickname(uint userId, string nickname)
        {
            Logger.Instance().Analytics("FriendRequestByNickname", "Friend",
                "UserId", userId.ToString(),
                "Nickname", nickname);
        }
    }
}
