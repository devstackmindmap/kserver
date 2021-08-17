using CommonProtocol;

namespace BattleLogic
{
    public class CardUseActionData
    {
        public Card UseCard;
        //public uint? NextCardStatId;
        //public uint ReplacedCardStatId;
        //public int ReplacedHandIndex;
        public ProtoTarget Target;
        public ReplaceCardInfo ReplaceCardInfo = new ReplaceCardInfo();
        public ProtoTargetUnitInfo PerformerUnitInfo;
    }
}