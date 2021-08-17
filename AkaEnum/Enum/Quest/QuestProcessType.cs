namespace AkaEnum
{
    public enum QuestProcessType
    {
        None = 0,
        Completed,
        ActionNormalAttack,
        ActionSpell,
        ActionBuff,
        ActionNormalAttackDamage,

        Death = 9,
        ClientSide = 10,

        KnightLeagueVictory = 1001,
        UnitLevelUp = 1002,
        SkillLevelUp = 1003,
        GettingGold = 1004,   //not used
        FinalRankPoint = 1005,
        FinalVirtualRankPoint = 1006,  //인포터 업적
        StageClear = 1007,
        DailyKnightLeagueVictory = 1008, 

        DynamicQuest = 2001,

        BattleActionStatus = 10000,
        CardUseWith1 = 10001,
        CardUseWith2 = 10002,
        CardUseWith3 = 10003,
        CardUseWith4 = 10004,
        CardUseWith5 = 10005,
        CardUseWith6 = 10006,
        CardUseWith7 = 10007,
        CardUseWith8 = 10008,
        CardUseWith9 = 10009,
        CardUseWith10 = 10010,

        CardUseWithNormal = 10021,
        CardUseWithSpecial = 10022,
        CardUseWithUltimate = 10023,

        EmoticonUse = 10024,
        UnitDeath = 10025,
        EnterExtensionTime = 10027,
        EnterBoostTime = 10028,
        KillUnit = 10029,
        CounterAttack = 10030,
        BlockingAttack = 10031,

        UnitActionDealing = 11001,
        UnitActionUseCard = 11002,
        UnitActionVictory = 11003,


        CardActionUse = 12001,
        
        BattleActionStatusEnd = 19999,

    }
}
