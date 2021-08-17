namespace AkaEnum.Battle
{
    public enum ActionStatusType
    {
        None = 0,
        CardUseWith1 = 1,
        CardUseWith2 = 2,
        CardUseWith3 = 3,
        CardUseWith4 = 4,
        CardUseWith5 = 5,
        CardUseWith6 = 6,
        CardUseWith7 = 7,
        CardUseWith8 = 8,
        CardUseWith9 = 9,
        CardUseWith10 = 10,

        CardUseWithNormal = 21,
        CardUseWithSpecial = 22,
        CardUseWithUltimate = 23,

        EmoticonUse = 24,

        UnitDeath = 25,

        EnterExtensionTime = 27,
        EnterBoostTime = 28,
        KillUnit = 29,

        CounterAttack = 30,
        BlockingAttack = 31,

        UnitAction = 1000,
        UnitActionDealing = 1001,
        UnitActionUseCard = 1002,
        UnitActionVictory = 1003,


        CardAction = 2000,
        CardActionUse = 2001,

        EndOfAction = 9999,
    }
}
