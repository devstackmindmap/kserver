namespace AkaData
{
    public class DataUnitRankPoint
    {
        public uint UnitRankLevelId { get; set; }
        public int NeedRankPointForNextLevelUp { get; set; }
        public int AiWinPoint { get; set; }
        public int AiLosePoint { get; set; }
        public int WinPoint { get; set; }
        public int LosePoint { get; set; }
        public uint RewardId { get; set; }
    }
}