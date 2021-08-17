using System;

namespace AkaLogger.Matching
{
    public sealed class LogTryMatching
    {
        public void Log(uint userId, string roomId, byte battleType, int deckNum , byte deckModeType, string battleServerIp, string matchingKey, string memberKey, int teamRankPoint, int userRankPoint)
        {
            Logger.Instance().Analytics("TryMatching", "Matching",
                "UserId", userId.ToString(),
                "RoomId", roomId,
                "BattleType", battleType.ToString(),
                "DeckNum", deckNum.ToString(),
                "DeckModeType", deckModeType.ToString(),
                "BattleServer", battleServerIp,
                "MatchingKey", matchingKey,
                "MemberKey", memberKey,
                "TeamRankPoint", teamRankPoint.ToString(),
                "UserRankPoint", userRankPoint.ToString());
        }
    }
}
