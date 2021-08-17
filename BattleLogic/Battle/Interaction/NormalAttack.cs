using AkaEnum;
using AkaLogger;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using AkaData;
using AkaEnum.Battle;

namespace BattleLogic
{
    public sealed class NormalAttack : Interaction
    {
        private readonly Player _myPlayer;
        private readonly Player _enemyPlayer;

        public NormalAttack(Unit performer, Player myPlayer, Player enemyPlayer) : base(performer)
        {
            _myPlayer = myPlayer;
            _enemyPlayer = enemyPlayer;
        }

        public bool IsCanDoAction()
        {
            return _performer.IsCanDoAction();
        }

        public BattleActionAttackResult DoAttack()
        {
            if (_myPlayer.IsEndBattle())
                return new BattleActionAttackResult() { IsDoing = false };

            var targets = new List<Unit>();

            if (_performer.IsContainNextBuff(SkillEffectType.BUFF_NEXT_ATTACK_ATK_UP_AND_ALL_TARGET))
                targets.AddRange(_enemyPlayer.Units.Values);
            else
                targets.Add(GetUnitByAggro(_enemyPlayer.Units));

            var protoNormalAttack = GetProtoNormalAttack();
            CheckBuffNextTargetAddBuffState(targets, protoNormalAttack);

            var isIgnoreSteelCounter = _performer.IsContainNextBuff(SkillEffectType.BUFF_NEXT_ATTACK_IGNORE_STEEL_COUNTER);
            var normalAttackOrNextAttackAnimationLength = DoDamage(targets, protoNormalAttack.TargetUnitInfos, _performer.UnitData.UnitStatus.AttackDelay, _performer.UnitData.UnitStatus.FirstTakeDamage);

            var counterAnimationLength = 0;
            if (isIgnoreSteelCounter)
            {
                protoNormalAttack.CounterUnitInfos = new List<ProtoCounterTargetUnitInfo>();
            }
            else
            {
                var counterUnitInfoAndAddAnimationLength = GetCounterUnitInfo(targets, protoNormalAttack.TargetUnitInfos);
                protoNormalAttack.CounterUnitInfos = counterUnitInfoAndAddAnimationLength.protoCounterTargetUnitInfos;
                counterAnimationLength = counterUnitInfoAndAddAnimationLength.addAnimationLength;
            }

            var finishTime = GetFinishTime(normalAttackOrNextAttackAnimationLength, counterAnimationLength);
            protoNormalAttack.FinishTime = finishTime.Ticks;

            _performer.UnitPassive.PassiveConditionCheck(PassiveConditionType.PerNormalAttack);
            for (var i = 0; i < targets.Count; i++)
            {
                targets[i].UnitPassive.PassiveConditionCheck(PassiveConditionType.PerGottenNormalAttack);
                targets[i].UnitPassive.PassiveConditionCheck(PassiveConditionType.PerGottenAttack);
            }

            CheckGrowthAtk(protoNormalAttack);
            S2CManager.SendNormalAttack(_performer, protoNormalAttack);
            return new BattleActionAttackResult()
            {
                IsDoing = true,
                Performer = _performer,
                FinishTime = finishTime
            };
        }

        private ProtoNormalAttack GetProtoNormalAttack()
        {
            return new ProtoNormalAttack
            {
                MessageType = MessageType.AttackUnit,
                PerformerPlayerType = _performer.PlayerType,
                PerformerUnitId = _performer.UnitData.UnitIdentifier.UnitId,
                TargetUnitInfos = new List<ProtoTargetUnitInfo>()
            };
        }

        private int DoDamage(List<Unit> targets, List<ProtoTargetUnitInfo> targetUnitInfos, int attackDelay, int firstTakeDamage)
        {
            var isShieldIgnore = _performer.IsContainNextBuff(SkillEffectType.BUFF_NEXT_ATTACK_SHIELD_IGNORE);
            var isRemoveBuffs = _performer.IsContainNextBuff(SkillEffectType.BUFF_NEXT_ATTACK_DISAPPEARANCE);
            var animationLength = 0;

            for (var i = 0; i < targets.Count; i++)
            {
                var attackerAtk = BuffStateCalculator.GetAtkWithConditions(_performer.UnitData.UnitStatus.Atk, _performer, SkillEffectTypes.NormalAttackAtkByAttackers, firstTakeDamage, targets[i]);
                var attackerAtkAndAnimationLength = BuffStateCalculator.GetAtkWithNexts(attackerAtk, _performer, SkillEffectTypes.NextBuffAtkByAttackers, targets[i], firstTakeDamage);
                var damage = BuffStateCalculator.GetAtkWithConditions(attackerAtkAndAnimationLength.attackerAtk, targets[i], SkillEffectTypes.NormalAttackByTargets, firstTakeDamage, _performer);
                var isCritical = BuffStateCalculator.GetIsCriticalWithNexts(_performer, SkillEffectTypes.NormalAttackByAttackersAtCriProbabilityUp, targets[i], firstTakeDamage);

                animationLength = attackerAtkAndAnimationLength.animationLength;

                damage = BuffStateCalculator.GetAtkWithConditions(damage, _performer, SkillEffectTypes.NormalAttackDamageByAttackers, firstTakeDamage, targets[i]);
                damage *= _performer.UnitData.UnitStatus.AddDamageRateToNormalAttack;
                damage = CriticalDamageCalculator.GetCriticalDamageAsNormalAttack(_performer, targets[i], isCritical, damage, firstTakeDamage);

                var logPrevHp = targets[i].UnitData.UnitStatus.Hp;
                var logPrevShield = targets[i].Shield;
                var logPoisonDamage = targets[i].PoisonInfo.GetCurrentPoisonDamage(attackDelay);

                var decreaseHpInfo = targets[i].DecreaseHp(_performer, damage, DamageReasonType.NormalAttack, attackDelay, isShieldIgnore);

                targetUnitInfos.Add(new ProtoTargetUnitInfo()
                {
                    PlayerType = targets[i].PlayerType,
                    UnitId = targets[i].UnitData.UnitIdentifier.UnitId,
                    DecreaseHpInfo = decreaseHpInfo,
                    Hp = targets[i].UnitData.UnitStatus.Hp,
                    Shields = targets[i].UnitShields.GetShieldInfos(),
                    IsCritical = isCritical,
                    IsShieldIgnore = isShieldIgnore,
                    RemoveBuffs = GetRemoveBuffs(isRemoveBuffs, targets[i])
                });

                Log.Battle.DoAttack.Log(_myPlayer.Battle.RoomId, _myPlayer.PlayerIdentifier.Player, _performer.UnitData.UnitIdentifier.UnitId,
                    targets[i].UnitData.UnitIdentifier.UnitId, logPrevHp, targets[i].UnitData.UnitStatus.Hp, decreaseHpInfo.Damage, isCritical, logPrevShield, targets[i].Shield, isShieldIgnore, logPoisonDamage);
            }

            _performer.RemoveNextBuff(SkillEffectTypes.NextBuffAtkByAttackers);
            _performer.RemoveNextBuff(SkillEffectTypes.NextBuffCriRateByAttackers);
            _performer.RemoveNextBuff(SkillEffectTypes.NormalAttackByAttackersAtCriProbabilityUp);

            if (isShieldIgnore)
                _performer.RemoveNextBuff(SkillEffectType.BUFF_NEXT_ATTACK_SHIELD_IGNORE);

            if (isRemoveBuffs)
                _performer.RemoveNextBuff(SkillEffectType.BUFF_NEXT_ATTACK_DISAPPEARANCE);

            //Logger.Instance().Info($"[BNUM:{_myPlayer.Battle.GetBattleNum()}] [{_myPlayer.PlayerIdentifier.Player.ToString()}] " +
            //    $"[Unit] [Attack] [After]  [ID:{_performer.UnitData.UnitIdentifier.UnitId}] " +
            //    $"[Target:{target.UnitData.UnitIdentifier.UnitId} HP:{target.UnitData.UnitStatus.Hp} Damage:{targetUnitInfo.Damage}]");



            return animationLength;
        }

        private List<SkillEffectType> GetRemoveBuffs(bool isRemoveBuffs, Unit target)
        {
            if (isRemoveBuffs == false)
                return null;

            var nextBuff = _performer.NextBuffs.GetNextBuff(SkillEffectType.BUFF_NEXT_ATTACK_DISAPPEARANCE);
            var goodBadType = (StateGoodBadType)nextBuff.DataSkillOption.Value3;
            var ignoreType = GetRemoveIgnoreTypes(target);

            return target.RemoveConditionBuffs(ignoreType, goodBadType);
        }

        private List<SkillEffectType> GetRemoveIgnoreTypes(Unit target)
        {
            var ignoreTypes = new List<SkillEffectType>();

            if (target.IsContainConditionBuffAndNotValidRemove(SkillEffectType.BUFF_STATE_IMMUNE_ELECTRIC_DISAPPEARANCE))
                ignoreTypes.Add(SkillEffectType.BUFF_STATE_IMMUNE_ELECTRIC_DISAPPEARANCE);

            if (target.IsContainConditionBuffAndNotValidRemove(SkillEffectType.BUFF_STATE_CURSER))
                ignoreTypes.Add(SkillEffectType.BUFF_STATE_CURSER);

            return ignoreTypes;
        }

        private (List<ProtoCounterTargetUnitInfo> protoCounterTargetUnitInfos, int addAnimationLength)
            GetCounterUnitInfo(List<Unit> targets, List<ProtoTargetUnitInfo> targetUnitInfos)
        {
            var counterTargetUnitInfos = new List<ProtoCounterTargetUnitInfo>();
            var addAnimationLength = 0;

            var ignoreCounterBuff = _performer.NextBuffs.GetNextBuff(SkillEffectType.BUFF_NEXT_ATTACK_IGNORE_STEEL_COUNTER);
            var isValidNextBuffIgnoreCounter = ignoreCounterBuff != null;

            if (isValidNextBuffIgnoreCounter)
            {
                _performer.RemoveNextBuff(SkillEffectType.BUFF_NEXT_ATTACK_IGNORE_STEEL_COUNTER);
                return (counterTargetUnitInfos, addAnimationLength);
            }

            for (var i = 0; i < targets.Count; i++)
            {
                if (targets[i].IsDeath())
                    continue;

                var counterBuff = GetCounterBuff(targets[i]);
                if (counterBuff == null)
                    continue;

                float damage = targetUnitInfos[i].DecreaseHpInfo.Damage;
                counterBuff.CalculateValue(ref damage, null);

                DecreaseCounterBuff(counterBuff, targets[i]);

                var decreaseHpInfo = _performer.DecreaseHp(targets[i], (int)damage, DamageReasonType.NormalCounterAttack, 0);
                targets[i].UnitPassive.PassiveConditionCheck(PassiveConditionType.PerCounterAttack);
                _performer.UnitPassive.PassiveConditionCheck(PassiveConditionType.PerGottenCounter);

                counterTargetUnitInfos.Add(new ProtoCounterTargetUnitInfo()
                {
                    TargetUnitInfo = new ProtoTargetUnitInfo()
                    {
                        PlayerType = targets[i].PlayerType,
                        UnitId = targets[i].UnitData.UnitIdentifier.UnitId,
                        DecreaseHpInfo = decreaseHpInfo,
                        Hp = _performer.UnitData.UnitStatus.Hp,
                        Shields = _performer.UnitShields.GetShieldInfos()
                    },
                    SkillOptionId = counterBuff.SkillOptionId
                });
                addAnimationLength += (counterBuff as BuffSkillCounter).AnimationLength;

                targets[i].MyPlayer.ActionLog.IncreaseStatus(ActionStatusType.CounterAttack);
            }

            return (counterTargetUnitInfos, addAnimationLength);
        }

        private IBuffSkill GetCounterBuff(Unit target)
        {
            var counterAllBuff = target.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_COUNTER_ALL);
            var counterBuff = target.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_COUNTER);
            var isValidCounterAllBuff = counterAllBuff != null && counterAllBuff.IsValid();
            var isValidCounterBuff = counterBuff != null && counterBuff.IsValid();

            if (isValidCounterAllBuff && isValidCounterBuff)
            {
                var now = DateTime.UtcNow;
                var counterAllRemainTime = counterAllBuff.EndDateTime - now;
                var counterRemainTime = counterBuff.EndDateTime - now;

                return counterAllRemainTime < counterRemainTime ? counterAllBuff : counterBuff;
            }

            if (isValidCounterAllBuff)
                return counterAllBuff;

            if (isValidCounterBuff)
                return counterBuff;

            return null;
        }

        private DateTime GetFinishTime(int normalAttackOrNextAttackAnimationLength, int counterAnimationLength)
        {
            var totalAnimationLength
                = (normalAttackOrNextAttackAnimationLength == 0
                ? _performer.UnitData.UnitStatus.AttackDelay
                : normalAttackOrNextAttackAnimationLength) + counterAnimationLength;

            return DateTime.UtcNow.AddMilliseconds(totalAnimationLength);
        }

        private void DecreaseCounterBuff(IBuffSkill counterBuff, Unit target)
        {
            counterBuff.DecreaseCount();
            if (counterBuff.IsValid() == false)
            {
                target.RemoveConditionBuff(SkillEffectType.BUFF_STATE_COUNTER);
            }
        }

        private void CheckGrowthAtk(ProtoNormalAttack normalAttack)
        {
            var growthAtk = _performer.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_GROWTH_ATK);
            if (growthAtk == null)
                return;

            if (growthAtk.IsValid() == false)
            {
                _performer.RemoveConditionBuff(SkillEffectType.BUFF_STATE_GROWTH_ATK);
                return;
            }

            _performer.UnitData.UnitStatus.GrowthAtk += (int)growthAtk.Value;
            normalAttack.GrowthAtk = (int)growthAtk.Value;
        }

        private void CheckBuffNextTargetAddBuffState(List<Unit> targets, ProtoNormalAttack protoNormalAttack)
        {
            var buff = _performer.NextBuffs.GetNextBuff(SkillEffectType.BUFF_NEXT_TARGET_ADD_BUFF_STATE) as NextBuffTargetAddBuffState;
            if (buff == null)
                return;

            var takeDamageTime = buff.TakeDamageTime == 0 ? _performer.UnitData.UnitStatus.FirstTakeDamage : 0;
            protoNormalAttack.BuffStates = new List<ProtoNormalAttackAddBuffState>();

            for (var i = 0; i < targets.Count; i++)
            {
                if (IsHasImmuneBuff(targets[i]))
                    continue;

                protoNormalAttack.BuffStates.Add(new ProtoNormalAttackAddBuffState() { SkillOptionIds = new List<uint>(), BuffStates = new List<BaseSkillProto>() });
                for (var j = 0; j < buff.SkillOptionIds.Count; j++)
                {
                    var dataSkillOption = Data.GetSkillOption(buff.SkillOptionIds[j]);
                    if (IsHasBuffImmuneBuff(targets[i], dataSkillOption.SkillEffectType))
                        continue;

                    if (dataSkillOption.SkillEffectType == SkillEffectType.BUFF_STATE_ELECTRIC_SHOCK)
                    {
                        if (IsHasBuffImmuneElectric(targets[i]))
                            continue;
                    }

                    var skillProto = targets[i].AddConditionBuff(dataSkillOption.SkillEffectType, dataSkillOption, _performer, targets[i], takeDamageTime);
                    protoNormalAttack.BuffStates[i].SkillOptionIds.Add(dataSkillOption.SkillOptionId);
                    protoNormalAttack.BuffStates[i].BuffStates.Add(skillProto);
                }
            }
        }

        private bool IsHasImmuneBuff(Unit target)
        {
            var immuneBuff = target.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_IMMUNE);
            if (immuneBuff == null)
                return false;

            if (immuneBuff.IsValid() == false)
            {
                target.RemoveConditionBuff(SkillEffectType.BUFF_STATE_IMMUNE);
                return false;
            }

            return true;
        }

        private bool IsHasBuffImmuneBuff(Unit target, SkillEffectType skillEffectType)
        {
            if (Data.GetState(skillEffectType).GoodBad == StateGoodBadType.Bad)
                return false;

            if (target.IsContainConditionBuffAndNotValidRemove(SkillEffectType.BUFF_STATE_BUFF_IMMUNE))
                return true;

            if (target.IsContainConditionBuffAndNotValidRemove(SkillEffectType.BUFF_STATE_CURSER))
            {
                if (SkillEffectTypes.CurserIgnore.Contains(skillEffectType))
                    return false;
                return true;
            }

            return false;
        }

        private bool IsHasBuffImmuneElectric(Unit target)
        {
            var buffImmuneElectric = target.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_IMMUNE_ELECTRIC_DISAPPEARANCE);
            if (buffImmuneElectric == null)
                return false;

            if (buffImmuneElectric.IsValid() == false)
            {
                target.RemoveConditionBuff(SkillEffectType.BUFF_STATE_IMMUNE_ELECTRIC_DISAPPEARANCE);
                return false;
            }

            return true;
        }
    }
}
