using AkaEnum;
using AkaUtility;
using System;

namespace BattleLogic
{
    public static class BuffStateCalculator
    {
        public static float GetAtkWithConditions(float attackerAtk, Unit unit, SkillEffectType[] checkBuffs, double delayMilliseconds, Unit target = null)
        {
            foreach (var buff in checkBuffs)
            {
                if (unit.UnitBuffs.Buffs.ContainsKey(buff) == false)
                    continue;

                if (unit.UnitBuffs.Buffs[buff].IsValid(delayMilliseconds))
                {
                    unit.UnitBuffs.Buffs[buff].CalculateValue(ref attackerAtk, target);
                    unit.UnitBuffs.Buffs[buff].DecreaseCount();

                    if (unit.UnitBuffs.Buffs[buff].IsValid(delayMilliseconds) == false)
                        unit.RemoveConditionBuff(buff);
                }
                else
                {
                    unit.RemoveConditionBuff(buff);
                }
            }
            return attackerAtk;
        }

        public static float GetAtkWithConditions(float attackerAtk, Unit unit, SkillEffectType[] checkBuffs)
        {
            foreach (var buff in checkBuffs)
            {
                if (unit.UnitBuffs.Buffs.ContainsKey(buff) == false)
                    continue;

                if (unit.UnitBuffs.Buffs[buff].IsValid())
                {
                    unit.UnitBuffs.Buffs[buff].CalculateValue(ref attackerAtk, null);

                    if (unit.UnitBuffs.Buffs[buff].IsValid() == false)
                        unit.RemoveConditionBuff(buff);
                }
                else
                {
                    unit.RemoveConditionBuff(buff);
                }
            }
            return attackerAtk;
        }

        public static (float attackerAtk, int animationLength) GetAtkWithNexts(float attackerAtk, Unit unit, SkillEffectType[] checkBuffs, Unit target, double delayMilliseconds)
        {
            int animationLength = 0;
            foreach (var buff in checkBuffs)
            {
                if (unit.NextBuffs.Buffs.ContainsKey(buff) == false)
                    continue;

                unit.NextBuffs.Buffs[buff].CalculateValue(ref attackerAtk, target, unit, delayMilliseconds);
                if (animationLength == 0 || unit.NextBuffs.Buffs[buff].DataSkillOption.AnimationType != AnimationType.Attack)
                    animationLength = unit.NextBuffs.Buffs[buff].AnimationLength;
            }

            // 애니메이션이 들어간 다음 공격은 기획상 1개만 들어가있음
            return (attackerAtk, animationLength);
        }

        public static bool GetIsCriticalWithNexts(Unit unit, SkillEffectType[] checkBuffs, Unit target, double delayMilliseconds)
        {
            var criticalPability = unit.UnitData.UnitStatus.CriticalRate;
            foreach (var buff in checkBuffs)
            {
                if (unit.NextBuffs.Buffs.ContainsKey(buff) == false)
                    continue;

                unit.NextBuffs.Buffs[buff].CalculateValue(ref criticalPability, target, unit, delayMilliseconds);
            }

            return AkaRandom.Random.IsSuccess(criticalPability);
        }

        public static bool GetIsCriticalWithNextBuffSkills(Unit unit, SkillEffectType[] checkBuffs, Unit target, double delayMilliseconds)
        {
            var criticalPability = unit.UnitData.UnitStatus.CriticalRate;
            foreach (var buff in checkBuffs)
            {
                if (unit.NextBuffSkills.Buffs.ContainsKey(buff) == false)
                    continue;

                unit.NextBuffSkills.Buffs[buff].CalculateValue(ref criticalPability, target, unit, delayMilliseconds);
            }

            return AkaRandom.Random.IsSuccess(criticalPability);
        }

        public static float GetAddDamageRateByApplyUnitSpecies(Unit performer, Unit target)
        {
            switch (target.UnitData.UnitIdentifier.UnitType)
            {
                case UnitType.Human:
                    return performer.UnitData.UnitStatus.AddDamageRateToHuman;
                case UnitType.Kimera:
                    return performer.UnitData.UnitStatus.AddDamageRateToKimera;
                case UnitType.Mechanical:
                    return performer.UnitData.UnitStatus.AddDamageRateToMechanical;
                case UnitType.Beast:
                    return performer.UnitData.UnitStatus.AddDamageRateToBeast;
                default:
                    throw new Exception("Invalid UnitType. Check the unit data.");
            }
        }
    }
}