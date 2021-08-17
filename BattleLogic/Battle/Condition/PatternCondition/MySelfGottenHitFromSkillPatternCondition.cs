using AkaData;
using AkaEnum;
using AkaUtility;

namespace BattleLogic
{
    [PatternCondition(PatternType = ActionPatternType.PerMyselfGottenHitCountFromSkill)]
    public sealed class MySelfGottenHitFromSkillPatternCondition : MonsterPatternCondition
    {
        public MySelfGottenHitFromSkillPatternCondition(uint unitId, DataMonsterPatternCondition origin, AkaEnum.Battle.PlayerType player, AkaEnum.Battle.PlayerType enemyPlayer) : base(unitId, origin, player, enemyPlayer)
        {
        }

        public override void UpdateCondition(AkaEnum.Battle.PlayerType player, uint monsterId, DamageReasonType reasonType)
        {
            if (IsMe(player, monsterId) && reasonType.In(DamageReasonType.SkillCurHpRateAttack, DamageReasonType.SkillMaxHpRateAttack, DamageReasonType.SkillLoseHpRateAttack,
                    DamageReasonType.SkillAttack, DamageReasonType.SkillAsMineAttack, DamageReasonType.SkillShieldAttack, DamageReasonType.SkillCounterAttack))
            {
                CurrentActionPatternValue++;
            }
        }
    }
}