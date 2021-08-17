using System;
using AkaEnum;

namespace BattleLogic
{
    public class SkillAttribute : Attribute
    {
        public SkillEffectType[] EffectTypes;

        public SkillAttribute(params SkillEffectType[] effectTypes)
        {
            EffectTypes = effectTypes;
        }
    }
}
