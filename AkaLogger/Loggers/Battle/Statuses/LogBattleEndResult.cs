using System;
using System.Collections.Generic;

namespace AkaLogger.Battle
{
    public sealed class LogBattleEndResult
    {
        public void Log(string roomId, byte battleType, string winner, uint player1UserId, uint player2UserId, 
            string player1Nickname, string player2Nickname, 
            DateTime startDateTime, DateTime endDateTime,
            IEnumerable<uint> player1Units, IEnumerable<uint> player2Units,
            IEnumerable<uint> player1UnitLevels, IEnumerable<uint> player2UnitLevels,
            IEnumerable<uint> player1Cards, IEnumerable<uint> player2Cards,
            uint player1RankLevel, uint player2RankLevel)
        {

            Logger.Instance().Analytics("BattleEndResult", "BattleStatus",
                "RoomId", roomId,
                "BattleType", battleType.ToString(),
                "Winner", winner,
                "Player1UserId", player1UserId.ToString(),
                "Player2UserId", player2UserId.ToString(),
                "Player1RankLevel", player1RankLevel.ToString(),
                "Player2RankLevel", player2RankLevel.ToString(),
                "Player1NickName", player1Nickname,
                "Player2NickName", player2Nickname,
                "Player1Units", string.Join(",", player1Units),
                "Player2Units", string.Join(",", player2Units),
                "Player1UnitLevels", string.Join(",", player1UnitLevels),
                "Player2UnitLevels", string.Join(",", player2UnitLevels),
                "Player1Cards", string.Join(",", player1Cards),
                "Player2Cards", string.Join(",", player2Cards),
                "StartTime", startDateTime.ToLog(),
                "EndTime", endDateTime.ToLog());
        }


        public void Log(string roomId, byte battleType, string winner, bool isRoundResult, 
                        uint player1UserId, uint player2UserId, string player1Nickname, string player2Nickname, DateTime startDateTime, DateTime endDateTime,
                        IEnumerable<uint> player1Units, IEnumerable<uint> player2Units, IEnumerable<uint> player1UnitLevels, IEnumerable<uint> player1Cards,
                        uint player1RankLevel)
        {
            Logger.Instance().Analytics("BattleEndResult", "BattleStatus",
                "RoomId", roomId,
                "BattleType", battleType.ToString(),
                "Winner", winner,
                "Player1UserId", player1UserId.ToString(),
                "Player2UserId", player2UserId.ToString(),
                "Player1RankLevel", player1RankLevel.ToString(),
                "Player1NickName", player1Nickname,
                "Player2NickName", player2Nickname,
                "Player1Units", string.Join(",", player1Units),
                "Player2Units", string.Join(",", player2Units),
                "Player1UnitLevels", string.Join(",", player1UnitLevels),
                "Player1Cards", string.Join(",", player1Cards),
                "IsRoundResult", isRoundResult.ToString(),
                "StartTime", startDateTime.ToLog(),
                "EndTime", endDateTime.ToLog());
        }

        public void Log(string roomId, byte battleType, string winner, uint currentStageRoundId, uint stageLevelId, uint round, bool isRoundResult,
                        uint player1UserId, uint player2UserId, string player1Nickname, string player2Nickname, DateTime startDateTime, DateTime endDateTime,
                        IEnumerable<uint> player1Units, IEnumerable<uint> player2Units, IEnumerable<uint> player1UnitLevels, IEnumerable<uint> player1Cards,
                        uint player1RankLevel)
        {
            Logger.Instance().Analytics("BattleEndResult", "BattleStatus",
                "RoomId", roomId,
                "BattleType", battleType.ToString(),
                "Winner", winner,
                "Player1UserId", player1UserId.ToString(),
                "Player2UserId", player2UserId.ToString(),
                "Player1RankLevel", player1RankLevel.ToString(),
                "Player1NickName", player1Nickname,
                "Player2NickName", player2Nickname,
                "Player1Units", string.Join(",", player1Units),
                "Player2Units", string.Join(",", player2Units),
                "Player1UnitLevels", string.Join(",", player1UnitLevels),
                "Player1Cards", string.Join(",", player1Cards),
                "StageRoundId", currentStageRoundId.ToString(),
                "StageLevel", stageLevelId.ToString(),
                "StageRound", round.ToString(),
                "IsRoundResult", isRoundResult.ToString(),
                "StartTime", startDateTime.ToLog(),
                "EndTime", endDateTime.ToLog());
        }

    }
}
