using CommonProtocol;

namespace BattleLogic
{
    public class ReplaceCardInfo
    {
        public Card ReplacedCard;
        public uint? NextCardStatId;
        public int ReplacedHandIndex = 0;
    }
}