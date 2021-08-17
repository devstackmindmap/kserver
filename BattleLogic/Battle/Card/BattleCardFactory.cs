using AkaData;

namespace BattleLogic
{
    public class BattleCardFactory
    {
        public static BattleCard CreateBattleCard(Deck playerDeck)
        {
            return playerDeck is AiDeck ?  new MonsterBattleCard(playerDeck.Cards) : new BattleCard(playerDeck.Cards);
        }
    }
}
