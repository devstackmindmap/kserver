using System;

namespace AkaLogger.Matching
{
    public sealed class LogMatchingResult
    {
        public void Log(uint userId, byte battleType, string resultMessage)
        {
            Logger.Instance().Analytics("MatchingFailed", "Matching",
                "UserId", userId.ToString(),
                "BattleType", battleType.ToString(),
                "Description", resultMessage);
        }
        public void Log(string message, string category)
        {
            Logger.Instance().Analytics("MatchingResult", "Matching",
                "Message", message,
                "Category", category);
        }

        public void Log(string roomId, byte battleType, int teamRankPoint, int userRankPoint, int enemyRankPoint, int enemyUserRankPoint, string battleServerIp, string memberKey, string matchingKey)
        {
            Logger.Instance().Analytics("MatchingSuccess", "Matching",
                "RoomId", roomId,
                "BattleType", battleType.ToString(),
                "EnemyType", "Human",
                "TeamRankPoint", teamRankPoint.ToString(),
                "UserRankPoint", userRankPoint.ToString(),
                "EnemyTeamRankPoint", enemyRankPoint.ToString(),
                "EnemyUserRankPoint", enemyUserRankPoint.ToString(),
                "BattleServer", battleServerIp,
                "MatchingKey", matchingKey,
                "MemberKey", memberKey);
        }

        public void Log(byte battleType, int teamRankPoint, int userRankPoint, int enemyRankPoint, int enemyUserRankPoint, string battleServerIp, string memberKey, string matchingKey)
        {
            Logger.Instance().Analytics("MatchingSuccess", "Matching",
                "BattleType", battleType.ToString(),
                "EnemyType", "Ai",
                "TeamRankPoint", teamRankPoint.ToString(),
                "UserRankPoint", userRankPoint.ToString(),
                "EnemyTeamRankPoint", enemyRankPoint.ToString(),
                "EnemyUserRankPoint", enemyUserRankPoint.ToString(),
                "BattleServer", battleServerIp,
                "MatchingKey", matchingKey,
                "MemberKey", memberKey);
        }
    }
}
