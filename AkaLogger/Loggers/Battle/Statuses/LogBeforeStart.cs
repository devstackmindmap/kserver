using System;
using System.Collections.Generic;

namespace AkaLogger.Battle
{
    public sealed class LogBeforeStart
    {
        public void Log(string roomId, uint stageRoundId, byte battleType, uint player1UserId, uint player2UserId,
            List<uint> player1CardStatIds, IEnumerable<uint> player1UnitIds, IEnumerable<uint> player1Levels,
            List<uint> player2CardStatIds, IEnumerable<uint> player2UnitIds, IEnumerable<uint> player2Levels)
        {
            Logger.Instance().Analytics("BeforeStart", "BattleStatus",
                "RoomId", roomId,
                "BattleType", battleType.ToString(),
                "StageRoundId", stageRoundId,
                "Player1UserId", player1UserId,
                "Player2UserId", player2UserId,
                "Player1CardStatIds", string.Join(",", player1CardStatIds),
                "Player1UnitIds", string.Join(",", player1UnitIds),
                "Player1UnitLevels", string.Join(",", player1Levels),
                "Player2CardStatIds", string.Join(",", player2CardStatIds),
                "Player2UnitIds", string.Join(",", player2UnitIds),
                "Player2UnitLevels", string.Join(",", player2Levels)
                );
        }
    }
}
