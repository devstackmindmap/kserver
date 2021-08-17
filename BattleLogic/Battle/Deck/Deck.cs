using AkaEnum;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleLogic
{
    public class Deck
    {
        public Queue<Card> Cards = new Queue<Card>();
        public Dictionary<int, Unit> Units = new Dictionary<int, Unit>();
    }
}
