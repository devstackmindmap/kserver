using AkaData;
using AkaEnum;
using System;

namespace BattleLogic
{
    public class PatternCard : Card
    {
        public uint PatternId { get; }

        public PatternCard(DataCard dataCard, DataCardStat dataCardStat, uint patternId) : base(dataCard, dataCardStat)
        {
            PatternId = patternId;
        }

    }
}
