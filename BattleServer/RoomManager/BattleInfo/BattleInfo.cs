namespace BattleServer
{
    public class BattleInfo : IBattleInfo
    {
        public byte DeckNum;
        public AkaEnum.Battle.BattleType BattleType { get; set; }
        public uint UserId;

        public string BattleServerIp;
        public string SessionId;

        public uint StageLevelId;
        public uint StageRoundId;
    }
}
