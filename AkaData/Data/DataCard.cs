using AkaEnum;

namespace AkaData
{
    public class DataCard
    {
        public uint CardId { get; set; }
        public CardRarity CardRarity { get; set; }
        public uint UnitId { get; set; }
        public uint UseLevel { get; set; }
        public CardType CardType { get; set; }
        public UnlockType UnlockType { get; set; }
        public bool IsDisappear { get; set; }
        public uint RewardId { get; set; }
    }
}