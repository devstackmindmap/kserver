using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_CRITICAL_RATE_AND_DMG)]
    public sealed class BuffSkillCriticalRateAndDmg : BuffSkill
    {
        private float _criticalRate;
        private float _criticalDmg;

        public override float Value => _criticalRate;

        public override void DoSkill(DataSkillOption option, Unit performer, Unit target, int bulletTime)
        {
            base.DoSkill(option, performer, target, bulletTime);

            _criticalRate = option.Value2;
            _criticalDmg = option.Value3 * 0.01f;
            target.UnitData.UnitStatus.CriticalRate += _criticalRate;
            target.UnitData.UnitStatus.CriticalDamageRate += _criticalDmg;
        }

        public override IBuffSkill Clone()
        {
            return new BuffSkillCriticalRateAndDmg();
        }

        public override void CalculateValue(ref float value, Unit target)
        {
        }

        public override void BuffEnd()
        {
            _unit.UnitData.UnitStatus.CriticalRateOrigin -= _criticalRate;
            _unit.UnitData.UnitStatus.CriticalDamageRateOrigin -= _criticalDmg;
        }
    }
}