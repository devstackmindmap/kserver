using AkaData;
using AkaEnum;
using AkaLogger;
using AkaUtility;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleLogic
{
    public sealed class SkillBehavior : Interaction
    {
        public SkillBehavior(Unit performer) : base(performer)
        {
        }

        //cardUseActionData
        //public BattleActionSkillResult DoSkill(Card card, ProtoTarget target, uint replacedCardStatId = 0, int replacedHandIndex = 0, uint? nextCardStatId = 0)
        public BattleActionSkillResult DoSkill(CardUseActionData cardData)
        {
            if (!_performer.IsCanDoAction())
                return new BattleActionSkillResult() { IsDoing = false };

            //var skillId = SkillConditionCalculator.GetSkillId(_performer, target, card.DataCardStat);
            var skillId = SkillConditionCalculator.GetSkillId(_performer, cardData.Target, cardData.UseCard.DataCardStat);
            var skill = Data.GetSkill(skillId, _performer.UnitData.UnitIdentifier.UnitInitial);

            if (skill.SkillData.SkillType != SkillType.Passive)
                _performer.UnitPassive.PassiveConditionCheck(PassiveConditionType.PerUseSkill);

            var allSkillOptionTargets = TargetMaker.GetTargets(_performer.MyPlayer.Battle, _performer, skill.SkillData.SkillOptions[0], cardData.Target);
            if (allSkillOptionTargets == null || allSkillOptionTargets.Count == 0)
            {
                InvalidateCardReplacedCard(cardData);
                S2CManager.SendInvalidateCardUse(_performer, cardData.ReplaceCardInfo);
                return new BattleActionSkillResult() { IsDoing = false };
            }

            var nowTime = DateTime.UtcNow;
            DateTime catchBattleProgressTime = GetCatchBattleProgressTime(nowTime, skill);
            if (IsBulletCardRarity(cardData.UseCard.CardRarity))
            {
                _performer.MyPlayer.Battle.AddBulletTime(skill.AnimationData.BulletTime);
                _performer.BattleHelper.Pause();
                skill.AnimationData.TakeDamageTime = 0;
            }
            else
            {
                skill.AnimationData.BulletTime = 0;
            }

            var ignoreNextBuffSkills = new List<SkillEffectType>();
            var protoSkill = GetProtoSkill(cardData.UseCard.CardStatId, nowTime.Ticks, skillId, cardData.ReplaceCardInfo);
            DoSkillSecondStep(cardData, skill, protoSkill.SkillOptionResults, nowTime, ignoreNextBuffSkills);
            PassiveEnd(cardData.UseCard, skill);
            if (skill.SkillData.SkillType == SkillType.Passive)
                ignoreNextBuffSkills.Add(SkillEffectType.BUFF_NEXT_SKILL_ATTACK_CRITICAL_RATE);
            _performer.RemoveNextBuffSkills(ignoreNextBuffSkills);
            var counterUnitInfoAndAddAnimationLength = GetCounterUnitInfo(protoSkill.SkillOptionResults, allSkillOptionTargets);
            protoSkill.CounterUnitInfos = counterUnitInfoAndAddAnimationLength.protoTargetUnitInfos;
            protoSkill.CounterAnimationType = counterUnitInfoAndAddAnimationLength.animationType;
            S2CManager.SendSkill(_performer, protoSkill);

            return new BattleActionSkillResult()
            {
                IsDoing = true,
                BattleHelper = _performer.BattleHelper,
                BulletTime = skill.AnimationData.BulletTime,
                CatchBattleProgressTime = catchBattleProgressTime.AddMilliseconds(counterUnitInfoAndAddAnimationLength.addAnimationLength)
            };
        }

        private void InvalidateCardReplacedCard(CardUseActionData cardData)
        {
            if (cardData.ReplaceCardInfo.ReplacedCard == null || cardData.ReplaceCardInfo.ReplacedCard.IsDeath == false)
                return;

            cardData.ReplaceCardInfo.ReplacedCard = _performer.MyPlayer.Battle.GetBattleController().GetHandCard
                (_performer.MyPlayer.PlayerIdentifier.Player, cardData.ReplaceCardInfo.ReplacedHandIndex);
            cardData.ReplaceCardInfo.NextCardStatId = _performer.MyPlayer.Battle.GetBattleController().GetNexCardStatId(_performer.MyPlayer.PlayerIdentifier.Player);
        }

        private DateTime GetCatchBattleProgressTime(DateTime nowTime, Skill skill)
        {
            return nowTime.AddMilliseconds(skill.AnimationData.BulletTime);
        }

        private bool IsBulletCardRarity(CardRarity cardRarity)
        {
            return cardRarity == CardRarity.ULTIMATE || cardRarity == CardRarity.SPECIAL;
        }

        private ProtoSkill GetProtoSkill(uint cardStatId, long bulletTimeStart, uint skillId, ReplaceCardInfo replaceCardInfo)
        {
            uint? nextCardStatId = replaceCardInfo.NextCardStatId;

            if (replaceCardInfo.ReplacedCard != null && replaceCardInfo.ReplacedCard.IsDeath)
            {
                replaceCardInfo.ReplacedCard = _performer.MyPlayer.Battle.GetBattleController().GetHandCard
                    (_performer.MyPlayer.PlayerIdentifier.Player, replaceCardInfo.ReplacedHandIndex);
                nextCardStatId = _performer.MyPlayer.Battle.GetBattleController().GetNexCardStatId(_performer.MyPlayer.PlayerIdentifier.Player);
            }

            uint replacedCardStatId = replaceCardInfo.ReplacedCard == null ? 0 : replaceCardInfo.ReplacedCard.CardStatId;

            return new ProtoSkill()
            {
                MessageType = MessageType.Skill,
                PerformerPlayerType = _performer.PlayerType,
                PerformerUnitId = _performer.UnitData.UnitIdentifier.UnitId,
                SkillOptionResults = new List<ProtoSkillOptionResult>(),
                UsedCardStatId = cardStatId,
                BulletTimeStart = bulletTimeStart,
                SkillId = skillId,
                ReplacedCardStatId = replacedCardStatId,
                ReplacedHandIndex = replaceCardInfo.ReplacedHandIndex,
                NextCardStatId = nextCardStatId
            };
        }

        private (List<ProtoTargetUnitInfo> protoTargetUnitInfos, int addAnimationLength, AnimationType animationType)
            GetCounterUnitInfo(List<ProtoSkillOptionResult> results, List<Unit> targets)
        {
            var retTargetUnitInfo = new List<ProtoTargetUnitInfo>();
            var animationLength = 0;
            var animationType = AnimationType.Attack;

            for (var i = 0; i < results.Count; i++)
            {
                var dataSkillOption = Data.GetSkillOption(results[i].SkillOptionId);
                if (SkillEffectTypes.SkillCounter.Contains(dataSkillOption.SkillEffectGroupType) == false)
                    continue;

                foreach (var skillTargetResult in results[i].SkillTargetResults)
                {
                    var protoSpellDamage = skillTargetResult as ProtoSpellDamage;
                    var target = GetTarget(targets, protoSpellDamage.TargetUnitId);
                    if (target == null || target.IsDeath())
                        continue;

                    var counterSkillAttack = GetSkillCounter(target);
                    if (counterSkillAttack == null)
                        continue;

                    var damage = (float)protoSpellDamage.DecreaseHpInfo.Damage;
                    counterSkillAttack.CalculateValue(ref damage, null);

                    DecreaseCounterBuff(counterSkillAttack, target);

                    var decreaseHpInfo = _performer.DecreaseHp(target, (int)damage, DamageReasonType.SkillCounterAttack, 0);
                    target.UnitPassive.PassiveConditionCheck(PassiveConditionType.PerCounterAttack);
                    _performer.UnitPassive.PassiveConditionCheck(PassiveConditionType.PerGottenCounter);

                    retTargetUnitInfo.Add(new ProtoTargetUnitInfo()
                    {
                        PlayerType = target.PlayerType,
                        UnitId = target.UnitData.UnitIdentifier.UnitId,
                        DecreaseHpInfo = decreaseHpInfo,
                        Hp = _performer.UnitData.UnitStatus.Hp,
                        Shields = _performer.UnitShields.GetShieldInfos(),
                        IsCritical = false
                    });

                    animationLength += counterSkillAttack.AnimationLength;
                    animationType = Data.GetSkillOption(counterSkillAttack.SkillOptionId).AnimationType;
                }
                break;
            }

            return (retTargetUnitInfo, animationLength, animationType);
        }

        private BuffSkillCounter GetSkillCounter(Unit target)
        {
            var counterSkillAttack = target.UnitBuffs.GetBuffSkill(SkillEffectType.BUFF_STATE_COUNTER_SKILL_ATTACK) as BuffSkillCounter;
            if (counterSkillAttack != null && counterSkillAttack.IsValid())
                return counterSkillAttack;

            var counterAll = target.UnitBuffs.GetBuffSkill(SkillEffectType.BUFF_STATE_COUNTER_ALL) as BuffSkillCounter;
            if (counterAll != null && counterAll.IsValid())
                return counterAll;

            return null;
        }

        private Unit GetTarget(List<Unit> targets, uint unitId)
        {
            for (var i = 0; i < targets.Count; i++)
            {
                if (targets[i].UnitData.UnitIdentifier.UnitId == unitId)
                    return targets[i];
            }

            return null;
        }

        private void DecreaseCounterBuff(IBuffSkill counterBuff, Unit target)
        {
            counterBuff.DecreaseCount();
            if (counterBuff.IsValid() == false)
                target.RemoveConditionBuff(counterBuff.SkillEffectType);
        }

        private void DoSkillSecondStep(CardUseActionData card, Skill skill, List<ProtoSkillOptionResult> optionResults, DateTime nowDateTime, List<SkillEffectType> ignoreNextBuffSkills)
        {
            IEnumerable<Unit> targets = new List<Unit>();
            foreach (var skillOption in skill.SkillData.SkillOptions)
            {
                if (skillOption.TargetType != TargetType.Chain)
                {
                    targets = TargetMaker.GetTargets(_performer.MyPlayer.Battle, _performer, skillOption, card.Target);
                    if (targets == null)
                        continue;

                    targets = targets.Where(unit => unit.IsDeath() == false);
                }

                var optionResult = new ProtoSkillOptionResult
                {
                    SkillTargetResults = new List<BaseSkillProto>(),
                    SkillOptionId = skillOption.SkillOptionId
                };

                switch (skillOption.SkillEffectGroupType)
                {
                    case SkillEffectGroupType.BuffState:
                        DoBuff(targets, skillOption, optionResult, skill.AnimationData.BulletTime);
                        break;
                    case SkillEffectGroupType.Spell:
                    case SkillEffectGroupType.SpellDmg:
                    case SkillEffectGroupType.SpellFixingDmg:
                        if (skillOption.SkillEffectGroupType.In(SkillEffectGroupType.SpellDmg, SkillEffectGroupType.SpellFixingDmg))
                            _performer.UnitPassive.PassiveConditionCheck(PassiveConditionType.PerSkillAttack);
                        DoSpell(targets, skillOption, optionResult, card.UseCard, skill, nowDateTime);
                        break;
                    case SkillEffectGroupType.BuffNext:
                        DoNext(targets, skillOption, optionResult, skill.AnimationData.TakeDamageTime);
                        break;
                    case SkillEffectGroupType.BuffNextSkill:
                        DoNextSkill(targets, skillOption, optionResult, skill.AnimationData.TakeDamageTime);
                        ignoreNextBuffSkills.Add(skillOption.SkillEffectType);
                        break;
                }
                optionResults.Add(optionResult);
            }
        }

        private void DoBuff(IEnumerable<Unit> targets, DataSkillOption skillOption, ProtoSkillOptionResult optionResult, int bulletTime)
        {
            foreach (var targetUnit in targets)
            {
                if (AkaRandom.Random.IsSuccess(skillOption.Value4))
                {
                    var hasImmuneBuff = IsHasImmuneBuff(targetUnit, skillOption.SkillEffectType, SkillEffectType.BUFF_STATE_IMMUNE);
                    if (hasImmuneBuff)
                        continue;

                    if (skillOption.SkillEffectType == SkillEffectType.BUFF_STATE_ELECTRIC_SHOCK)
                    {
                        var hasImmuneElectricBuff = IsHasImmuneBuff(targetUnit, skillOption.SkillEffectType, SkillEffectType.BUFF_STATE_IMMUNE_ELECTRIC_DISAPPEARANCE);
                        if (hasImmuneElectricBuff)
                            continue;
                    }

                    var hasBuffImmuneBuff = IsHasBuffImmuneBuff(targetUnit, skillOption.SkillEffectType);
                    if (hasBuffImmuneBuff)
                        continue;

                    if (skillOption.SkillEffectType == SkillEffectType.BUFF_STATE_OFFERING)
                        RemoveOfferingBuff(targetUnit);

                    targetUnit.UnitPassive.PassiveConditionCheck(PassiveConditionType.PerGottenCondition);
                    var buffResult = targetUnit.AddConditionBuff(skillOption.SkillEffectType, skillOption, _performer, targetUnit, bulletTime);
                    optionResult.SkillTargetResults.Add(buffResult);
                }
                else
                {
                    //Logger.Instance().Info("Buff Fail");
                    //Log.Battle.SkillBehavior.BuffFailLog(_performer.MyPlayer.Battle.RoomId, _performer.PlayerType, targetUnit.PlayerType, _performer.UnitData.UnitIdentifier.UnitId, targetUnit.UnitData.UnitIdentifier.UnitId, skillOption.SkillOptionId);

                    optionResult.SkillTargetResults.Add(new ProtoBuffFail()
                    {
                        SkillEffectType = SkillEffectType.BUFF_FAIL,
                        TargetPlayerType = targetUnit.PlayerType,
                        TargetUnitId = targetUnit.UnitData.UnitIdentifier.UnitId
                    });
                }
            }
        }

        private bool IsHasImmuneBuff(Unit target, SkillEffectType useSkillEffectType, SkillEffectType immuneSkillEffectType)
        {
            if (useSkillEffectType == SkillEffectType.BUFF_STATE_IMMUNE)
                return false;

            var immuneBuff = target.GetConditionBuffSkill(immuneSkillEffectType);
            if (immuneBuff == null)
                return false;

            if (immuneBuff.IsValid() == false)
            {
                target.RemoveConditionBuff(immuneSkillEffectType);
                return false;
            }

            return true;
        }

        private bool IsHasBuffImmuneBuff(Unit target, SkillEffectType skillEffectType)
        {
            if (Data.GetState(skillEffectType).GoodBad == StateGoodBadType.Bad)
                return false;

            if (skillEffectType != SkillEffectType.BUFF_STATE_BUFF_IMMUNE && target.IsContainConditionBuffAndNotValidRemove(SkillEffectType.BUFF_STATE_BUFF_IMMUNE))
                return true;

            if (skillEffectType != SkillEffectType.BUFF_STATE_CURSER && target.IsContainConditionBuffAndNotValidRemove(SkillEffectType.BUFF_STATE_CURSER))
            {
                if (SkillEffectTypes.CurserIgnore.Contains(skillEffectType))
                    return false;

                return true;
            }

            return false;
        }

        private void RemoveOfferingBuff(Unit targetUnit)
        {
            var units = targetUnit.MyPlayer.Units;
            foreach (var unit in units)
            {
                if (unit.Value.UnitData.UnitIdentifier.UnitId == targetUnit.UnitData.UnitIdentifier.UnitId)
                    continue;

                var offeringBuff = unit.Value.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_OFFERING);
                if (offeringBuff == null)
                    continue;

                targetUnit.BattleHelper.EnqueueBuffEnd(new BattleActionBuffEnd(unit.Value, offeringBuff));
            }
        }

        private void DoSpell(IEnumerable<Unit> targets, DataSkillOption skillOption, ProtoSkillOptionResult optionResult,
            Card card, Skill skill, DateTime nowDateTime)
        {
            var isCritical = BuffStateCalculator.GetIsCriticalWithNextBuffSkills(_performer, SkillEffectTypes.NextBuffSkillCriRateByAttackers, null, 0);

            foreach (var targetUnit in targets)
            {
                if (AkaRandom.Random.IsSuccess(skillOption.Value4))
                {
                    if (IsImmuneShield(skillOption.SkillEffectType, targetUnit))
                        continue;

                    var logTargetPrevHp = targetUnit.UnitData.UnitStatus.Hp;
                    var logTargetPrevShield = targetUnit.UnitData.UnitStatus.Unit.UnitShields.Shield;

                    if (skillOption.SkillEffectGroupType == SkillEffectGroupType.SpellDmg)
                        targetUnit.GottenSkillAttack();

                    var spell = SkillFactory.CreateSpellSkill(skillOption.SkillEffectType);
                    optionResult.SkillTargetResults.Add(spell.DoSkill(skillOption, _performer, targetUnit, card, skill, nowDateTime, isCritical));

                    var logTargetHp = targetUnit.UnitData.UnitStatus.Hp;
                    var logTargetShield = targetUnit.UnitData.UnitStatus.Unit.UnitShields.Shield;

                    var logDamage = (logTargetHp + logTargetShield) - (logTargetPrevHp + logTargetPrevShield);


                    //Logger.Instance().Info("[Unit] [Spell] [After] [ID:{0}] [Target:{1} HP:{2}]", _performer.UnitData.UnitIdentifier.UnitId, targetUnit.UnitData.UnitIdentifier.UnitId, targetUnit.UnitData.UnitStatus.Hp);
                    Log.Battle.SkillBehavior.Log(_performer.MyPlayer.Battle.RoomId, _performer.PlayerType, _performer.UnitData.UnitIdentifier.UnitId,
                        targetUnit.UnitData.UnitIdentifier.UnitId, logTargetPrevHp, logTargetHp, logDamage, card.CardId, skill.SkillData.SkillId);
                }
                else
                {
                    //Logger.Instance().Info("Buff Fail");
                    //Log.Battle.SkillBehavior.SpellFailLog(_performer.MyPlayer.Battle.RoomId, _performer.PlayerType, 
                    //    _performer.UnitData.UnitIdentifier.UnitId, targetUnit.UnitData.UnitIdentifier.UnitId, card.CardId, skill.SkillData.SkillId);
                    optionResult.SkillTargetResults.Add(new ProtoBuffFail()
                    {
                        SkillEffectType = SkillEffectType.BUFF_FAIL,
                        TargetPlayerType = targetUnit.PlayerType,
                        TargetUnitId = targetUnit.UnitData.UnitIdentifier.UnitId
                    });
                }
            }
        }

        private void DoNext(IEnumerable<Unit> targets, DataSkillOption skillOption, ProtoSkillOptionResult optionResult, int takeDamageTime)
        {
            foreach (var targetUnit in targets)
            {
                targetUnit.AddNextBuff(skillOption.SkillEffectType, skillOption, GetAnimationLength(skillOption.AnimationType, _performer.UnitData.UnitIdentifier.UnitInitial), takeDamageTime);
                optionResult.SkillTargetResults.Add(new ProtoSkillEmpty()
                {
                    SkillEffectType = SkillEffectType.COMMON_NEXT_BUFFS,
                    TargetPlayerType = targetUnit.PlayerType,
                    TargetUnitId = targetUnit.UnitData.UnitIdentifier.UnitId
                });
            }
        }

        private void DoNextSkill(IEnumerable<Unit> targets, DataSkillOption skillOption, ProtoSkillOptionResult optionResult, int takeDamageTime)
        {
            foreach (var targetUnit in targets)
            {
                targetUnit.AddNextBuffSkill(skillOption.SkillEffectType, skillOption, GetAnimationLength(skillOption.AnimationType, _performer.UnitData.UnitIdentifier.UnitInitial), takeDamageTime);
                optionResult.SkillTargetResults.Add(new ProtoSkillEmpty()
                {
                    SkillEffectType = SkillEffectType.COMMON_NEXT_BUFF_SKILLS,
                    TargetPlayerType = targetUnit.PlayerType,
                    TargetUnitId = targetUnit.UnitData.UnitIdentifier.UnitId
                });
            }
        }

        private int GetAnimationLength(AnimationType animationType, string unitInitial)
        {
            if (animationType == AnimationType.Empty || animationType == AnimationType.None)
                return 0;

            return Data.GetAnimationLength(unitInitial, animationType).Bullet;
        }

        private void PassiveEnd(Card card, Skill skill)
        {
            if (skill.SkillData.SkillType == SkillType.Passive)
            {
                card.Action();
            }
        }

        private bool IsImmuneShield(SkillEffectType skillEffectType, Unit target)
        {
            if (skillEffectType != SkillEffectType.SPELL_SHIELD)
                return false;

            if (target.IsContainConditionBuffAndNotValidRemove(SkillEffectType.BUFF_STATE_CURSER))
                return true;

            return false;
        }
    }
}
