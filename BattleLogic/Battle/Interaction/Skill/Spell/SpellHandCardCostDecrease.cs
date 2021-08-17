using AkaData;
using AkaEnum;
using CommonProtocol;
using System;
using System.Collections.Generic;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_HANDCARD_COST_DECREASE)]
    public class SpellHandCardCostDecrease : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var handIndex = new List<int>();
            var handCards = target.BattleHelper.GetHandCards(target.PlayerType);
            for (var i = 0; i < handCards.Count; i++)
            {
                if (handCards[i].UnitId != target.UnitData.UnitIdentifier.UnitId)
                    continue;

                handCards[i].DecreaseElixir = Math.Min(handCards[i].DecreaseElixir + (int)option.Value2, 10);
                handIndex.Add(i);
                AkaLogger.Logger.Instance().Debug($"[HandCardCost] Index:{i}, CardId:{handCards[i].CardId}, CardStatId:{handCards[i].DataCardStat}, Elixir:{handCards[i].Elixir}");
            }

            return new ProtoHandCardCostDecrease()
            {
                SkillEffectType = SkillEffectType.SPELL_HANDCARD_COST_DECREASE,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId,
                HandIndex = handIndex
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellHandCardCostDecrease();
        }
    }
}