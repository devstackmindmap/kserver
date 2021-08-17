using System;

namespace AkaLogger.Matching
{
    public sealed class LogCancel
    {
        public void Log(string roomId, string member, string matchingGroupKey, string userRankPoint)
        {
            Logger.Instance().Analytics("CancelSuccess", "Matching",
                "RoomId", roomId,
                "MemberKey", member,
                "MatchingKey", matchingGroupKey,
                "UserRankPoint", userRankPoint);
        }

        public void Log(string member)
        {
            Logger.Instance().Analytics("CancelFailed", "Matching",
                "MemberKey", member);
        }
    }
}
