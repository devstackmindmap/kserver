using AkaEnum;
using AkaUtility;
using System;
using System.Collections.Generic;

namespace BattleLogic
{
    public static class SkillFactory
    {
        private static Dictionary<SkillEffectType, IBuffSkill> _conditionBuffSkills;
        private static Dictionary<SkillEffectType, INextBuff> _nextBuffSkills;
        private static Dictionary<SkillEffectType, ISpellSkill> _spellSkills;


        static SkillFactory()
        {
            InitConditionBuffSkills();
            InitNextBuffSkills();
            InitSpellSkills();
        }


        #region ConditionBuff

        public static IBuffSkill CreateConditionBuffSkill(SkillEffectType effectType)
        {
            InitConditionBuffSkills();

            if (_conditionBuffSkills.ContainsKey(effectType) == false)
                throw new Exception("Invalid SkillEffectType : " + effectType);

            return _conditionBuffSkills[effectType].Clone();
        }

        private static void InitConditionBuffSkills()
        {
            if (_conditionBuffSkills != null)
                return;

            _conditionBuffSkills = new Dictionary<SkillEffectType, IBuffSkill>(SkillEffectTypeComparer.Comparer);

            var types = AttributeCreator.GetTypes(typeof(BuffSkill));

            for (var i = 0; i < types.Count; i++)
            {
                var type = types[i];

                var attributes = type.GetCustomAttributes(typeof(SkillAttribute), false);
                if (AttributeCreator.IsAttributesEmpty(attributes, typeof(SkillAttribute)))
                    continue;

                AddConditionBuff(attributes[0] as SkillAttribute, type);
            }
        }

        private static void AddConditionBuff(SkillAttribute attribute, Type type)
        {
            var buff = Activator.CreateInstance(type) as BuffSkill;
            for (var i = 0; i < attribute.EffectTypes.Length; i++)
            {
                if (_conditionBuffSkills.ContainsKey(attribute.EffectTypes[i]))
                    continue;

                _conditionBuffSkills.Add(attribute.EffectTypes[i], buff);
            }
        }

        #endregion

        #region NextBuff

        public static INextBuff CreateNextBuffSkill(SkillEffectType effectType)
        {
            InitNextBuffSkills();

            if (_nextBuffSkills.ContainsKey(effectType) == false)
                throw new Exception("Invalid SkillEffectType : " + effectType);

            return _nextBuffSkills[effectType].Clone();
        }

        private static void InitNextBuffSkills()
        {
            if (_nextBuffSkills != null)
                return;

            _nextBuffSkills = new Dictionary<SkillEffectType, INextBuff>(SkillEffectTypeComparer.Comparer);

            var types = AttributeCreator.GetTypes(typeof(NextBuff));

            for (var i = 0; i < types.Count; i++)
            {
                var type = types[i];

                var attributes = type.GetCustomAttributes(typeof(SkillAttribute), false);
                if (AttributeCreator.IsAttributesEmpty(attributes, typeof(SkillAttribute)))
                    continue;

                AddNextBuff(attributes[0] as SkillAttribute, type);
            }
        }

        private static void AddNextBuff(SkillAttribute attribute, Type type)
        {
            var buff = Activator.CreateInstance(type) as NextBuff;
            for (var i = 0; i < attribute.EffectTypes.Length; i++)
            {
                if (_nextBuffSkills.ContainsKey(attribute.EffectTypes[i]))
                    continue;

                _nextBuffSkills.Add(attribute.EffectTypes[i], buff);
            }
        }

        #endregion

        #region Spell

        public static ISpellSkill CreateSpellSkill(SkillEffectType effectType)
        {
            InitSpellSkills();

            if (_spellSkills.ContainsKey(effectType) == false)
                throw new Exception("Invalid SkillEffectType : " + effectType);

            return _spellSkills[effectType].Clone();
        }

        private static void InitSpellSkills()
        {
            if (_spellSkills != null)
                return;

            _spellSkills = new Dictionary<SkillEffectType, ISpellSkill>(SkillEffectTypeComparer.Comparer);

            var types = AttributeCreator.GetTypes(typeof(ISpellSkill));

            for (var i = 0; i < types.Count; i++)
            {
                var type = types[i];

                var attributes = type.GetCustomAttributes(typeof(SkillAttribute), false);
                if (AttributeCreator.IsAttributesEmpty(attributes, typeof(SkillAttribute)))
                    continue;

                AddSpell(attributes[0] as SkillAttribute, type);
            }
        }

        private static void AddSpell(SkillAttribute attribute, Type type)
        {
            var spell = Activator.CreateInstance(type) as ISpellSkill;
            for (var i = 0; i < attribute.EffectTypes.Length; i++)
            {
                if (_spellSkills.ContainsKey(attribute.EffectTypes[i]))
                    continue;

                _spellSkills.Add(attribute.EffectTypes[i], spell);
            }
        }

        #endregion
    }
}
