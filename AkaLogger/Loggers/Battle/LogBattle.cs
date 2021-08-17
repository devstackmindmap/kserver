namespace AkaLogger.Battle
{
    public sealed class LogBattle
    {
        public readonly LogBeforeStart BeforeStart = new LogBeforeStart();
        public readonly LogStart Start = new LogStart();
        public readonly LogStartExtension StartExtension = new LogStartExtension();
        public readonly LogStartBooster StartBooster = new LogStartBooster();
        public readonly LogBattleEndResult BattleEndResult = new LogBattleEndResult();
        //public readonly LogEnqueue Enqueue = new LogEnqueue();
        public readonly LogDoAttack DoAttack = new LogDoAttack();

        //public readonly LogCreateRoom CreateRoom = new LogCreateRoom();
        public readonly LogEnterRoom EnterRoom = new LogEnterRoom();
        public readonly LogRetreat Retreat = new LogRetreat();
        public readonly LogReEnterRoom ReEnterRoom = new LogReEnterRoom();
        //public readonly LogClosedSession ClosedSession = new LogClosedSession();
        public readonly LogCardUse CardUse = new LogCardUse();
        //public readonly LogRunPattern RunPattern = new LogRunPattern();
        //public readonly LogShield Shiled = new LogShield();
        public readonly LogSkillBehavior SkillBehavior = new LogSkillBehavior();
        //public readonly LogBuffEndTime BuffEndTime = new LogBuffEndTime();

        public readonly LogBattleResultRedisFail BattleResultRedisFail = new LogBattleResultRedisFail();
    }
}
