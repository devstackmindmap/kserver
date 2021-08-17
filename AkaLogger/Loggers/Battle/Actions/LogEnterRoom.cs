namespace AkaLogger.Battle
{
    public sealed class LogEnterRoom
    {
        public void LogForPvP(uint userId, byte battleType, string roomId)
        {
            Logger.Instance().Analytics("EnterPvPRoom", "Action",
                "UserId", userId.ToString(),
                "BattleType", battleType.ToString(),
                "RoomId", roomId);
        }

        public void LogForPvE(uint userId, byte battleType, string roomId)
        {
            Logger.Instance().Analytics("EnterPvERoom", "Action",
                "UserId", userId.ToString(),
                "BattleType", battleType.ToString(),
                "RoomId", roomId);
        }
        public void LogForEnterRoom(uint userId, byte battleType, string roomId)
        {
            Logger.Instance().Analytics("EnterRoom", "Action",
                "UserId", userId.ToString(),
                "BattleType", battleType.ToString(),
                "RoomId", roomId);
        }

    }
}
