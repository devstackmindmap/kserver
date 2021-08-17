namespace AkaLogger.Battle
{
    public sealed class LogReEnterRoom
    {
        //public void Log(uint userId, string reason)
        //{
        //    Logger.Instance().Analytics("ReEnterRoom", "Action",
        //        "UserId", userId.ToString(),
        //        "Type", "EnterFailed",
        //        "Reason", reason);
        //}

        //public void Log(uint userId, byte battleType, string roomId)
        //{
        //    Logger.Instance().Analytics("ReEnterRoom", "Action",
        //        "UserId", userId.ToString(),
        //        "Type", "EnterSuccess",
        //        "BattleType", battleType.ToString(),
        //        "RoomId", roomId);
        //}

        public void LogTryReEnterRoom(uint userId)
        {
            Logger.Instance().Analytics("TryReEnterRoom", "Action",
                "UserId", userId.ToString());
        }
    }
}
