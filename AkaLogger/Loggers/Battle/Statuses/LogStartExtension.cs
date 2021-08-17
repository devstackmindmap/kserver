using System;

namespace AkaLogger.Battle
{
    public sealed class LogStartExtension
    {
        public void Log(string roomId, byte battleType, string unitCounts, uint player1UserId, uint player2UserId)
        {
            Logger.Instance().Analytics("StartExtension", "BattleStatus",
                "RoomId", roomId,
                "BattleType", battleType.ToString(),
                "UnitCount", unitCounts,
                "Player1UserId", player1UserId.ToString(),
                "Player2UserId", player2UserId.ToString());
        }
    }
}
