using AkaData;
using AkaEnum;
using System.Collections.Generic;
using AkaUtility;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_NEXT_TARGET_WEAK_ATK_UP, SkillEffectType.BUFF_NEXT_TARGET_BLIND_ATK_UP, SkillEffectType.BUFF_NEXT_TARGET_BERSERK_ATK_UP,
        SkillEffectType.BUFF_NEXT_TARGET_POISON_ATK_UP, SkillEffectType.BUFF_NEXT_TARGET_ATTENTION_ATK_UP, SkillEffectType.BUFF_NEXT_TARGET_ATTACKSPEED_ATK_UP,
        SkillEffectType.BUFF_NEXT_TARGET_STEALTH_ATK_UP)]
    public sealed class NextBuffTarget : NextBuff
    {
        private readonly Dictionary<SkillEffectType, SkillEffectType> _targetContainBuff = new Dictionary<SkillEffectType, SkillEffectType>(SkillEffectTypeComparer.Comparer)
        {
            { SkillEffectType.BUFF_NEXT_TARGET_WEAK_ATK_UP, SkillEffectType.BUFF_STATE_WEAK },
            { SkillEffectType.BUFF_NEXT_TARGET_BLIND_ATK_UP, SkillEffectType.BUFF_STATE_BLIND },
            { SkillEffectType.BUFF_NEXT_TARGET_BERSERK_ATK_UP, SkillEffectType.BUFF_STATE_BERSERK },
            { SkillEffectType.BUFF_NEXT_TARGET_POISON_ATK_UP, SkillEffectType.BUFF_STATE_POISON },
            { SkillEffectType.BUFF_NEXT_TARGET_ATTENTION_ATK_UP, SkillEffectType.BUFF_STATE_ATTENTION },
            { SkillEffectType.BUFF_NEXT_TARGET_ATTACKSPEED_ATK_UP, SkillEffectType.BUFF_STATE_ATTACKSPEED_RATE },
            { SkillEffectType.BUFF_NEXT_TARGET_STEALTH_ATK_UP, SkillEffectType.BUFF_STATE_STEALTH },
        };

        public override void DoSkill(DataSkillOption option, int animationLengh, int takeDamageTime)
        {
            base.DoSkill(option, animationLengh, takeDamageTime);

            _originValue = option.Value2;
            _currentValue = _originValue;
        }

        public override void CalculateValue(ref float value, Unit target, Unit performer, double delay)
        {
            if (_targetContainBuff.ContainsKey(SkillEffectType) == false)
                return;

            if (target.UnitBuffs.Buffs.ContainsKey(_targetContainBuff[SkillEffectType]) == false)
                return;

            value *= _currentValue;
        }

        public override INextBuff Clone()
        {
            return new NextBuffTarget();
        }
    }
}