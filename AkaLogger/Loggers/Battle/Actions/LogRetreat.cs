namespace AkaLogger.Battle
{
    public sealed class LogRetreat
    {
        public void Log(uint userId, byte battleType, string roomId)
        {
            Logger.Instance().Analytics("Retreat", "Action",
                "UserId", userId.ToString(),
                "BattleType", battleType.ToString(),
                "RoomId", roomId);
        }
    }
}
