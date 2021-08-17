using AkaData;
using AkaEnum;
using AkaUtility;

namespace BattleLogic
{
    [PatternCondition(PatternType = ActionPatternType.PerMyselfGottenHitCountFromAll)]
    public sealed class MySelfGottenHitFromAllPatternCondition : MonsterPatternCondition
    {
        public MySelfGottenHitFromAllPatternCondition(uint unitId, DataMonsterPatternCondition origin, AkaEnum.Battle.PlayerType player, AkaEnum.Battle.PlayerType enemyPlayer) : base(unitId, origin, player, enemyPlayer)
        {
        }

        public override void UpdateCondition(AkaEnum.Battle.PlayerType player, uint monsterId, DamageReasonType reasonType)
        {
            if (IsMe(player, monsterId) && reasonType.In(DamageReasonType.NormalAttack, DamageReasonType.NormalCounterAttack, DamageReasonType.SkillCurHpRateAttack, 
                    DamageReasonType.SkillMaxHpRateAttack, DamageReasonType.SkillLoseHpRateAttack, DamageReasonType.SkillAttack, DamageReasonType.SkillAsMineAttack, 
                    DamageReasonType.SkillShieldAttack, DamageReasonType.SkillCounterAttack))
            {
                CurrentActionPatternValue++;
            }
        }
    }
}