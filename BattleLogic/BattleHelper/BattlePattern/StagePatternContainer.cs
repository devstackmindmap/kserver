using AkaEnum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleLogic
{
    public class StagePatternContainer
    {
        private static readonly ActionPatternType[] _supportTimeActions = { ActionPatternType.PerTimeSecond };
        private static readonly ActionPatternType[] _supportSkillActions = { ActionPatternType.PerAllSkillUsingCount
                                                                            ,ActionPatternType.PerMyselfSkillUsingCount };
        private static readonly ActionPatternType[] _supportAttackActions = { ActionPatternType.PerMyselfHitCount };
        private static readonly ActionPatternType[] _supportGottenAttacks =
        {
            ActionPatternType.PerMyselfGottenHitCount,
            ActionPatternType.PerMyselfGottenHitCountFromSkill,
            ActionPatternType.PerMyselfGottenHitCountFromAll
        };
        private static readonly ActionPatternType[] _supportHpCheckActions = { ActionPatternType.MyselfHp };
        private static readonly ActionPatternType[] _supportDeadCountActions = { ActionPatternType.AllDeadUnitCount
                                                                                ,ActionPatternType.AllyDeadUnitCount};
        private static readonly ActionPatternType[] _supportHasIgnitionCheckActions = { ActionPatternType.HasIgnition };

        public AkaEnum.Battle.PlayerType PlayerType { get; }

        public List<Pattern> PatternsOfMonster { get; }

        public IEnumerable<MonsterPatternCondition> TimeActionConditions { get; private set; }
        public IEnumerable<MonsterPatternCondition> AttackActionConditions { get; private set; }
        public IEnumerable<MonsterPatternCondition> GottenAttackConditions { get; private set; }
        public IEnumerable<MonsterPatternCondition> HpCheckConditions { get; private set; }
        public IEnumerable<MonsterPatternCondition> DeadUnitCountConditions { get; private set; }
        public IEnumerable<MonsterPatternCondition> HasIgnitionConditions { get; private set; }


        public bool HasTimeAction => TimeActionConditions.Any();

        public StagePatternContainer(AkaEnum.Battle.PlayerType playerType, List<Pattern> patterns)
        {
            PatternsOfMonster = patterns;
            PlayerType = playerType;
            ResetConditions();
        }

        private void ResetConditions()
        {
            TimeActionConditions = GetActionConditions(_supportTimeActions);
            AttackActionConditions = GetActionConditions(_supportAttackActions);
            GottenAttackConditions = GetActionConditions(_supportGottenAttacks);
            HpCheckConditions = GetActionConditions(_supportHpCheckActions);
            DeadUnitCountConditions = GetActionConditions(_supportDeadCountActions);
            HasIgnitionConditions = GetActionConditions(_supportHasIgnitionCheckActions);
        }
        private IEnumerable<MonsterPatternCondition> GetActionConditions(ActionPatternType[] checker)
        {
            return PatternsOfMonster
                .Where(pattern => pattern.Deactived == false || pattern.HasActiveCondition())
                .SelectMany(pattern => pattern.CheckConditions)
                .Where(condition => typeof(BoolPatternCondition) != condition.GetType() && checker.Contains(condition.ConditionData.ActionPatternType));
        }

        public IEnumerable<MonsterPatternCondition> GetSkillConditions(AkaEnum.Battle.PlayerType player, uint unitId, Card exceptCard)
        {
            return PatternsOfMonster
                .Where(pattern => (pattern.Deactived == false || pattern.HasActiveCondition())
                                   && false == (exceptCard is PatternCard && PlayerType == player && pattern.UnitId == unitId && exceptCard.CardStatId == pattern.CardStatId && ((PatternCard)exceptCard).PatternId == pattern.PatternId))
                .SelectMany(pattern => pattern.CheckConditions)
                .Where(condition => typeof(BoolPatternCondition) != condition.GetType() && _supportSkillActions.Contains(condition.ConditionData.ActionPatternType));
        }

        internal void ReplacePattern(FlowPattern nextFlowPattern)
        {
            lock (this)
            {
                if (!nextFlowPattern.ParentPattern.Deactived)
                {
                    nextFlowPattern.ParentPattern.Deactived = true;

                    var nextPattern = PatternsOfMonster.Find(pattern => pattern.PatternId == nextFlowPattern.NextPatternId && pattern.UnitId == nextFlowPattern.UnitId);

                    PatternsOfMonster.OfType<FlowPattern>()
                        .Where(pattern => pattern.ParentPattern == nextPattern)
                        .All(pattern =>
                        {
                            pattern.ResetConditions();
                            return true;
                        });
                    nextPattern.ResetConditions();
                    ResetConditions();
                    nextPattern.Deactived = false;
                }
            }
        }
    }
}
