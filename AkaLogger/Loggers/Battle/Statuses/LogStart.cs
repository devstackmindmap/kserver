using System;

namespace AkaLogger.Battle
{
    public sealed class LogStart
    {

        public void Log(string roomId, byte battleType, uint player1UserId, uint player2UserId)
        {
            Logger.Instance().Analytics("Start", "BattleStatus",
                "RoomId", roomId,
                "BattleType", battleType.ToString(),
                "Player1UserId", player1UserId.ToString(),
                "Player2UserId", player2UserId.ToString());
        }
    }
}
