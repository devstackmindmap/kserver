using AkaEnum;
using AkaUtility;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using AkaData;

namespace BattleLogic
{
    public static class TargetMaker
    {
        public static List<Unit> GetTargets(Battle battle, Unit attacker, DataSkillOption skillOption, ProtoTarget target)
        {
            if ((skillOption.TargetType == TargetType.Target || skillOption.TargetType == TargetType.TargetAndSelf) && target != null)
                return GetTargets(battle, attacker, target, skillOption.TargetType);

            var candidateTargets = GetTargetGroupUnits(battle, attacker.PlayerType, skillOption.TargetGroupType);
            return GetTargetUnit(candidateTargets, attacker, skillOption.TargetType, skillOption.TargetId);
        }

        public static List<Unit> GetTargets(Battle battle, Unit attacker, ProtoTarget target, TargetGroupType groupType, TargetType targetType)
        {
            if ((targetType == TargetType.Target || targetType == TargetType.TargetAndSelf) && target != null)
                return GetTargets(battle, attacker, target, targetType);

            var candidateTargets = GetTargetGroupUnits(battle, attacker.PlayerType, groupType);
            return GetTargetUnit(candidateTargets, attacker, targetType, 0);
        }

        private static List<Unit> GetTargets(Battle battle, Unit attacker, ProtoTarget target, TargetType targetType)
        {
            switch (targetType)
            {
                case TargetType.Target:
                    if (battle.Players[target.PlayerType].Units.ContainsKey(target.UnitPositionIndex))
                        return new List<Unit> { battle.Players[target.PlayerType].Units[target.UnitPositionIndex] };
                    else
                        return null;
                case TargetType.TargetAndSelf:
                    if (battle.Players[target.PlayerType].Units.ContainsKey(target.UnitPositionIndex))
                        return new List<Unit> { attacker, battle.Players[target.PlayerType].Units[target.UnitPositionIndex] };
                    else
                        return null;
                default:
                    return null;
            }
        }

        private static List<Unit> GetTargetGroupUnits(Battle battle, AkaEnum.Battle.PlayerType myPlayerType, TargetGroupType targetGroupType)
        {
            List<Unit> targets = new List<Unit>();

            var enemyPlayerType = AkaEnum.Battle.PlayerType.Player1 == myPlayerType ? AkaEnum.Battle.PlayerType.Player2 : AkaEnum.Battle.PlayerType.Player1;

            if (targetGroupType == TargetGroupType.All)
            {
                targets.AddList(battle.Players[myPlayerType].Units);
                targets.AddList(battle.Players[enemyPlayerType].Units);
            }
            else if (targetGroupType == TargetGroupType.Ally)
            {
                targets.AddList(battle.Players[myPlayerType].Units);
            }
            else if (targetGroupType == TargetGroupType.Enemy)
            {
                targets.AddList(battle.Players[enemyPlayerType].Units);
            }
            else
            {
                throw new Exception("Check Target Maker Logic");
            }

            return targets;
        }

        private static List<Unit> GetTargetUnit(List<Unit> candidateUnits, Unit attacker, TargetType targetType, uint targetId)
        {
            switch (targetType)
            {
                case TargetType.All:
                    return candidateUnits;
                case TargetType.Random:
                    return new List<Unit> { candidateUnits[AkaRandom.Random.Next(0, candidateUnits.Count)] };
                case TargetType.AggroRandom:
                    return new List<Unit> { GetUnitByAggro(candidateUnits) };
                case TargetType.ForcedTarget:
                    var targets = candidateUnits.Where(unit => unit.UnitData.UnitIdentifier.UnitId == targetId).ToList();
                    if (targets.Any() == false)
                        targets = new List<Unit> { candidateUnits[AkaRandom.Random.Next(0, candidateUnits.Count)] };
                    return targets;
                case TargetType.Self:
                    return new List<Unit> { attacker };
                case TargetType.CurrentAtkHighest:
                    return new List<Unit> { GetUnitByCurrentAtkHighest(candidateUnits) };
                case TargetType.CurrentAtkLowest:
                    return new List<Unit> { GetUnitByCurrentAtkLowest(candidateUnits) };
                case TargetType.CurrentHpHighest:
                    return new List<Unit> { GetUnitByCurrentHpHighest(candidateUnits) };
                case TargetType.CurrentHpLowest:
                    return new List<Unit> { GetUnitByCurrentHpLowest(candidateUnits) };
                case TargetType.HasBuffTwoMore:
                    return new List<Unit> { GetUnitByHasBuffTwoMore(candidateUnits) };
                case TargetType.HasDebuffOneMore:
                    return new List<Unit> { GetUnitByHasDebuffOneMore(candidateUnits) };
                case TargetType.CurrentHpUnder30:
                    return new List<Unit> { GetUnitByCurrentHpUnder30(candidateUnits) };
                case TargetType.HasIgnitionOver30:
                    return new List<Unit> { GetUnitByHasIgnitionOver30(candidateUnits) };
                default:
                    throw new Exception("Invalid TargetType");
            }
        }

        private static Unit GetUnitByAggro(List<Unit> units)
        {
            var aggroSum = units.Sum(data => data.UnitData.UnitStatus.Aggro);
            var choiceValue = AkaRandom.Random.Next(0, aggroSum);

            var currentSum = 0;
            foreach (var unit in units)
            {
                currentSum += unit.UnitData.UnitStatus.Aggro;
                if (choiceValue < currentSum)
                    return unit;
            }

            throw new Exception("GetUnitByAggro Func Logic is wrong");
        }

        private static Unit GetUnitByCurrentAtkHighest(List<Unit> units)
        {
            var atk = 0f;
            Unit retUnit = null;

            foreach (var unit in units)
            {
                var calculateAtk = BuffStateCalculator.GetAtkWithConditions(unit.UnitData.UnitStatus.Atk, unit, SkillEffectTypes.NormalAttackAtkByAttackers);
                if (atk < calculateAtk)
                {
                    atk = calculateAtk;
                    retUnit = unit;
                }
            }

            return retUnit;
        }

        private static Unit GetUnitByCurrentAtkLowest(List<Unit> units)
        {
            var atk = 999999f;
            Unit retUnit = null;

            foreach (var unit in units)
            {
                var calculateAtk = BuffStateCalculator.GetAtkWithConditions(unit.UnitData.UnitStatus.Atk, unit, SkillEffectTypes.NormalAttackAtkByAttackers);
                if (atk > calculateAtk)
                {
                    atk = calculateAtk;
                    retUnit = unit;
                }
            }

            return retUnit;
        }

        private static Unit GetUnitByCurrentHpHighest(List<Unit> units)
        {
            var hp = 0;
            Unit retUnit = null;

            foreach (var unit in units)
            {
                if (hp < unit.UnitData.UnitStatus.Hp)
                {
                    hp = unit.UnitData.UnitStatus.Hp;
                    retUnit = unit;
                }
            }

            return retUnit;
        }

        private static Unit GetUnitByCurrentHpLowest(List<Unit> units)
        {
            var hp = 9999999;
            Unit retUnit = null;

            foreach (var unit in units)
            {
                if (hp > unit.UnitData.UnitStatus.Hp)
                {
                    hp = unit.UnitData.UnitStatus.Hp;
                    retUnit = unit;
                }
            }

            return retUnit;
        }

        private static Unit GetUnitByHasBuffTwoMore(List<Unit> units)
        {
            var hasBuffUnits = new List<Unit>();

            foreach (var unit in units)
            {
                var buffCount = 0;
                foreach (var buff in unit.UnitBuffs.Buffs)
                {
                    if (buff.Value.IsValid() == false)
                        continue;

                    buffCount++;
                }

                if (buffCount >= 2)
                    hasBuffUnits.Add(unit);
            }

            if (hasBuffUnits.Count == 0)
                return units[AkaRandom.Random.Next(0, units.Count)];

            return hasBuffUnits[AkaRandom.Random.Next(0, hasBuffUnits.Count)];
        }

        private static Unit GetUnitByHasDebuffOneMore(List<Unit> units)
        {
            var hasDebuffUnits = new List<Unit>();

            foreach (var unit in units)
            {
                var debuffCount = 0;
                foreach (var buff in unit.UnitBuffs.Buffs)
                {
                    if (buff.Value.IsValid() == false)
                        continue;

                    var goodBadType = Data.GetState(buff.Value.SkillEffectType).GoodBad;

                    if (goodBadType != StateGoodBadType.Bad)
                        continue;

                    debuffCount++;
                }

                if (debuffCount >= 1)
                    hasDebuffUnits.Add(unit);
            }

            if (hasDebuffUnits.Count == 0)
                return units[AkaRandom.Random.Next(0, units.Count)];

            return hasDebuffUnits[AkaRandom.Random.Next(0, hasDebuffUnits.Count)];
        }

        private static Unit GetUnitByCurrentHpUnder30(List<Unit> units)
        {
            var hp = 9999999;
            Unit retUnit = null;

            foreach (var unit in units)
            {
                if (unit.UnitData.UnitStatus.Hp * 100 / unit.UnitData.UnitStatus.MaxHp <= 30)
                    return unit;

                if (unit.UnitData.UnitStatus.Hp < hp)
                {
                    hp = unit.UnitData.UnitStatus.Hp;
                    retUnit = unit;
                }
            }

            return retUnit;
        }

        private static Unit GetUnitByHasIgnitionOver30(List<Unit> units)
        {
            foreach (var unit in units)
            {
                var conditionBuff = unit.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_IGNITION);
                if (conditionBuff == null || conditionBuff.IsValid() == false)
                    continue;

                if (conditionBuff.Value >= 30)
                    return unit;
            }

            return units[AkaRandom.Random.Next(0, units.Count)];
        }
    }
}
