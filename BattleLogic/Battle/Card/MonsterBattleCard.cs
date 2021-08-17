using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleLogic
{
    public class MonsterBattleCard : BattleCard
    {
        private List<PatternCard> _cards;

        public MonsterBattleCard(Queue<Card> cards)
            : base(new Queue<Card>())
        {
            _cards = new List<PatternCard>( cards.Cast< PatternCard>() );
        }

        public override void CardUseWithPattern( uint cardStatId, uint patternId, ProtoTarget performer, ProtoTarget target  )
        {
            if (!ValidateActions())
                throw new Exception("You need to Set Card Action");

            var selectCard = _cards.Find(card => card.CardStatId == cardStatId && card.PatternId == patternId);
            if (selectCard == null)
                return;


            var actionData = new CardUseActionData();

            actionData.UseCard = selectCard;
            actionData.Target = target;
            DoAction(performer.UnitPositionIndex, actionData);

            AkaLogger.Logger.Instance().Debug("[CardUse]  PerformerType:{0}, PerformerIndex:{1}, TargetType:{2}, TargetIndex:{3}",
                performer.PlayerType, performer.UnitPositionIndex,
                target.PlayerType, target.UnitPositionIndex);
        }

    }
}
