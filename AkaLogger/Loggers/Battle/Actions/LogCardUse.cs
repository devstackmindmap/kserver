using AkaEnum.Battle;

namespace AkaLogger.Battle
{
    public sealed class LogCardUse
    {
        public void Log(string roomId, uint userId, byte battleType, int handIndex, PlayerType PerformerType, int UnitPositionIndex, PlayerType TargetPlayerType, int TargetUnitPositionIndex
                        , uint performerUnitId, uint cardStatId, string elixirState, double currentElixir, int needElixir, string validationCardResult)
        {
            Logger.Instance().Analytics("CardUse", "Action",
                "UserId", userId.ToString(),
                "BattleType", battleType.ToString(),
                "HandIndex", handIndex,
                "PerformerType", PerformerType.ToString(),
                "UnitPositionIndex", UnitPositionIndex,
                "TargetPlayerType", TargetPlayerType.ToString(),
                "TargetUnitPositionIndex", TargetUnitPositionIndex,
                "PerformerUnitId", performerUnitId.ToString(),
                "CardStatId", cardStatId.ToString(),
                "ElixirCounterState", elixirState,
                "CurrentElixir", currentElixir.ToString(),
                "NeedElixir", needElixir.ToString(),
                "RoomId", roomId,
                "CardUseResult", validationCardResult);
        }

        //public void Log(int Index, uint CardId, uint replacedCardStatId)
        //{
        //    Logger.Instance().Analytics("CardMove", "Action",
        //        "Index", Index,
        //        "CardId", CardId,
        //        "replacedCardStatId", replacedCardStatId
        //        );
        //}

        //public void Log(int Index)
        //{
        //    Logger.Instance().Analytics("IsValidateCard", "Action",
        //        "Index",Index
        //        );
        //}

    }
}
