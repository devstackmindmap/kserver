using AkaData;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using System.Collections.Generic;

namespace BattleLogic
{
    public sealed class NextBuffs
    {
        private readonly Dictionary<SkillEffectType, INextBuff> _buffs = new Dictionary<SkillEffectType, INextBuff>(SkillEffectTypeComparer.Comparer);

        public Dictionary<SkillEffectType, INextBuff> Buffs => _buffs;

        public void AddBuff(SkillEffectType skillEffectType, DataSkillOption skillOption, int animationLength, int takeDamageTime)
        {
            if (_buffs.ContainsKey(skillEffectType))
            {
                _buffs[skillEffectType].Add(skillOption.Value2);
            }
            else
            {
                _buffs.Add(skillEffectType, SkillFactory.CreateNextBuffSkill(skillEffectType));
                _buffs[skillEffectType].DoSkill(skillOption, animationLength, takeDamageTime);
            }
        }

        public void RemoveBuff(params SkillEffectType[] skillEffectTypes)
        {
            for (var i = 0; i < skillEffectTypes.Length; i++)
            {
                _buffs.Remove(skillEffectTypes[i]);
            }
        }

        public INextBuff GetNextBuff(SkillEffectType skillEffectType)
        {
            if (_buffs.ContainsKey(skillEffectType) == false)
                return null;

            return _buffs[skillEffectType];
        }
    }
}
