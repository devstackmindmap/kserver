using AkaData;
using AkaEnum;
using System;

namespace BattleLogic
{
    public class Card
    {
        public uint UnitId;
        public uint CardId;
        public DataCardStat DataCardStat;
        public DataCard DataCard;
        public bool IsDeath;
        public Action Action;
        public int DecreaseElixir;
        public CardRarity CardRarity;
        public float HpCost;

        public ActionLog ActionLog { get; }

        private int _elixir;

        public uint CardStatId => DataCardStat.CardStatId;

        public Card(uint unitId, DataCardStat dataCardStat)
        {
            IsDeath = false;
            UnitId = unitId;
            CardId = dataCardStat.CardId;
            DataCardStat = dataCardStat;
            Elixir = dataCardStat.Elixir;
            HpCost = dataCardStat.HpCost;
            ActionLog = new SkillActionLog();
        }

        public Card(DataCard dataCard, DataCardStat dataCardStat)
        {
            IsDeath = false;
            UnitId = dataCard.UnitId;
            CardId = dataCard.CardId;
            DataCardStat = dataCardStat;
            Elixir = dataCardStat.Elixir;
            HpCost = dataCardStat.HpCost;
            CardRarity = dataCard.CardRarity;
            DataCard = dataCard;
            ActionLog = new SkillActionLog();
        }

        public int Elixir
        {
            get => Math.Max(_elixir + DecreaseElixir, 0);
            set => _elixir = value;
        }
    }
}
