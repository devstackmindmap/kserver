using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_CRITICAL_DMG)]
    public sealed class BuffSkillCriticalDamage : BuffSkill
    {
        private float _criticalValue;

        public override float Value => _criticalValue;

        public override void DoSkill(DataSkillOption option, Unit performer, Unit target, int bulletTime)
        {
            base.DoSkill(option, performer, target, bulletTime);

            _criticalValue = option.Value2;
            target.UnitData.UnitStatus.CriticalDamageRate += _criticalValue;
        }

        public override IBuffSkill Clone()
        {
            return new BuffSkillCriticalDamage();
        }

        public override void CalculateValue(ref float value, Unit target)
        {
        }

        public override void BuffEnd()
        {
            _unit.UnitData.UnitStatus.CriticalDamageRateOrigin -= _criticalValue;
        }
    }
}