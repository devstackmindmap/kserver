using AkaData;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using System;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_BASE_ADD_DMG_RATE_TO_UNIT_TYPE)]
    public class SpellBaseAddDamageRateToUnitType : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            if (option.Value3 == (int)UnitType.Human)
                target.UnitData.UnitStatus.AddDamageRateToHuman *= option.Value2;
            else if (option.Value3 == (int)UnitType.Kimera)
                target.UnitData.UnitStatus.AddDamageRateToKimera *= option.Value2;
            else if (option.Value3 == (int)UnitType.Mechanical)
                target.UnitData.UnitStatus.AddDamageRateToMechanical *= option.Value2;
            else if (option.Value3 == (int)UnitType.Beast)
                target.UnitData.UnitStatus.AddDamageRateToBeast *= option.Value2;
            else
                throw new Exception("Invalid UnitType. Check the unit data.");

            return new ProtoSpellBaseAddDamageRateToUnitType()
            {
                AddDamageRate = option.Value2,
                UnitType = (UnitType)option.Value3,
                SkillEffectType = SkillEffectType.SPELL_BASE_ADD_DMG_RATE_TO_UNIT_TYPE,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellBaseAddDamageRateToUnitType();
        }
    }
}