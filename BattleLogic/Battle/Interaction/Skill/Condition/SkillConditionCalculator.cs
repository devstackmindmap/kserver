using System;
using System.Collections.Generic;
using AkaData;
using AkaEnum;
using AkaUtility;
using CommonProtocol;

namespace BattleLogic
{
    public static class SkillConditionCalculator
    {
        private static Dictionary<SkillConditionType, BaseSkillCondition> _skillConditions;

        static SkillConditionCalculator()
        {
            InitSkillConditions();
        }

        public static uint GetSkillId(Unit performer, ProtoTarget target, DataCardStat cardStat)
        {
            if (cardStat.SkillConditionId == 0)
                return cardStat.SkillIdList[0];

            InitSkillConditions();

            var dataCondition = Data.GetSkillCondition(cardStat.SkillConditionId);
            var targets = TargetMaker.GetTargets(performer.MyPlayer.Battle, performer, target, dataCondition.TargetGroupType, dataCondition.TargetType);
            for (var i = 0; i < dataCondition.SkillConditionTypeList.Count; i++)
            {
                if (_skillConditions[dataCondition.SkillConditionTypeList[i]].IsConditionPass(performer, targets,
                    dataCondition.SkillConditionTypeList[i], dataCondition.SkillConditionValueList[i]))
                {
                    return cardStat.SkillIdList[i];
                }
            }

            return 0;
        }

        private static void InitSkillConditions()
        {
            if (_skillConditions != null)
                return;

            _skillConditions = new Dictionary<SkillConditionType, BaseSkillCondition>(SkillConditionTypeComparer.Comparer);

            var types = AttributeCreator.GetTypes(typeof(BaseSkillCondition));

            for (var i = 0; i < types.Count; i++)
            {
                var type = types[i];

                var attributes = type.GetCustomAttributes(typeof(SkillConditionAttribute), false);
                if (AttributeCreator.IsAttributesEmpty(attributes, typeof(SkillConditionAttribute)))
                    continue;

                AddSkillCondition(attributes[0] as SkillConditionAttribute, type);
            }
        }

        private static void AddSkillCondition(SkillConditionAttribute attribute, Type type)
        {
            var skillCondition = Activator.CreateInstance(type) as BaseSkillCondition;
            _skillConditions.Add(attribute.ConditionType, skillCondition);
        }
    }
}