using AkaEnum;
using BattleLogic;
using NUnit.Framework;

namespace BattleLogicTest
{
    public class SkillFactoryTest
    {
        [TestCase(SkillEffectType.BUFF_STATE_BLIND)]
        [TestCase(SkillEffectType.BUFF_STATE_STEEL)]
        [TestCase(SkillEffectType.BUFF_STATE_BERSERK)]
        [TestCase(SkillEffectType.BUFF_STATE_WEAK)]
        [TestCase(SkillEffectType.BUFF_STATE_WEAK_NORMAL)]
        [TestCase(SkillEffectType.BUFF_STATE_ATTENTION)]
        [TestCase(SkillEffectType.BUFF_STATE_ATTACKSPEED)]
        public void ContainBuffSkillTest(SkillEffectType effectType)
        {
            var buff = SkillFactory.CreateConditionBuffSkill(effectType);
            Assert.IsNotNull(buff);
        }

        [TestCase(SkillEffectType.SPELL_DMG_AS_MINE_ATK)]
        [TestCase(SkillEffectType.SPELL_SHIELD)]
        [TestCase(SkillEffectType.SPELL_FIXING_DMG_CUR_HP_RATE_ATTACK)]
        public void ContainSpellSkillTest(SkillEffectType effectType)
        {
            var buff = SkillFactory.CreateSpellSkill(effectType);
            Assert.IsNotNull(buff);
        }
    }
}