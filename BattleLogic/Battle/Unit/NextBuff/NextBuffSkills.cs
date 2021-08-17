using System.Collections.Generic;
using AkaData;
using AkaEnum;
using AkaUtility;

namespace BattleLogic
{
    public sealed class NextBuffSkills
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

        public void RemoveBuff(List<SkillEffectType> ignoreEffectTypes)
        {
            var removeTypes = new List<SkillEffectType>();
            foreach (var nextBuff in _buffs)
            {
                if (ignoreEffectTypes.Contains(nextBuff.Key))
                    continue;

                removeTypes.Add(nextBuff.Key);
            }

            for (var i = 0; i < removeTypes.Count; i++)
            {
                _buffs.Remove(removeTypes[i]);
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