using System;

namespace AkaLogger.Battle
{
    public sealed class LogStartBooster
    {
        public void Log(string roomId, byte battleType, uint player1UserId, uint player2UserId)
        {
            Logger.Instance().Analytics("StartBooster", "BattleStatus",
                "RoomId", roomId,
                "BattleType", battleType.ToString(),
                "Player1UserId", player1UserId.ToString(),
                "Player2UserId", player2UserId.ToString());
        }
    }
}
