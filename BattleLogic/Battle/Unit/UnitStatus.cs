using System;
using AkaData;
using AkaEnum;

namespace BattleLogic
{
    public class UnitStatus
    {

        public UnitStatus(AkaData.UnitStat unitStat, DataAnimationLength dataAnimationLength)
        {
            MaxHp = unitStat.Hp;
            Hp = unitStat.Hp;
            Atk = unitStat.Atk;
            Aggro = unitStat.Aggro;
            AttackSpeed = unitStat.AtkSpd;
            AttackDelay = dataAnimationLength.Bullet;
            FirstTakeDamage = dataAnimationLength.TakeDamage;
            CriticalRate = unitStat.CriRate;
            CriticalDamageRate = unitStat.CriDmgRate;
            AddDamageRateToBeast = 1;
            AddDamageRateToHuman = 1;
            AddDamageRateToKimera = 1;
            AddDamageRateToMechanical = 1;
            AddDamageRateToNormalAttack = 1;
        }

        public Unit Unit;
        public int GrowthMaxHp;
        private int _maxHp;

        public int MaxHp
        {
            get => _maxHp + GrowthMaxHp;
            set => _maxHp = value;
        }

        private int _hp;
        public int Hp
        {
            get => _hp;
            set
            {
                _hp = value;

                var immortalBuff = Unit?.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_IMMORTAL);
                if (immortalBuff != null)
                {
                    if (immortalBuff.IsValid() == false)
                        Unit.RemoveConditionBuff(SkillEffectType.BUFF_STATE_IMMORTAL);
                    else if (_hp <= 0)
                        _hp = 1;
                }
            }
        }
        public int GrowthAtk;
        private int _atk;
        public int Atk
        {
            get => _atk + GrowthAtk;
            set => _atk = value;
        }
        public int BasicAtk => _atk;

        public int AttackSpeed;

        public float GrowthCriticalRate;
        public float CriticalRateOrigin;
        public float CriticalRate
        {
            get
            {
                if (Unit == null)
                    return CriticalRateOrigin + GrowthCriticalRate;

                var criticalRateBuff = Unit.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_CRITICAL_RATE);
                if (criticalRateBuff?.IsValid() == false)
                    Unit.RemoveConditionBuff(SkillEffectType.BUFF_STATE_CRITICAL_RATE);

                var criticalRateAndDmgBuff = Unit.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_CRITICAL_RATE_AND_DMG);
                if (criticalRateAndDmgBuff?.IsValid() == false)
                    Unit.RemoveConditionBuff(SkillEffectType.BUFF_STATE_CRITICAL_RATE_AND_DMG);

                return CriticalRateOrigin + GrowthCriticalRate;
            }
            set => CriticalRateOrigin = value;
        }

        public float GrowthCriticalDamageRate;
        public float CriticalDamageRateOrigin;
        public float CriticalDamageRate
        {
            get
            {
                if (Unit == null)
                    return CriticalDamageRateOrigin + GrowthCriticalDamageRate;

                var criticalDmgBuff = Unit.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_CRITICAL_DMG);
                if (criticalDmgBuff?.IsValid() == false)
                    Unit.RemoveConditionBuff(SkillEffectType.BUFF_STATE_CRITICAL_DMG);

                var criticalRateAndDmgBuff = Unit.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_CRITICAL_RATE_AND_DMG);
                if (criticalRateAndDmgBuff?.IsValid() == false)
                    Unit.RemoveConditionBuff(SkillEffectType.BUFF_STATE_CRITICAL_RATE_AND_DMG);

                return CriticalDamageRateOrigin + GrowthCriticalDamageRate;
            }
            set => CriticalDamageRateOrigin = value;
        }

        public int GrowthAggro;
        public int AggroOrigin;
        public int Aggro
        {
            get
            {
                if (Unit == null)
                    return AggroOrigin;

                var aggro = 0;
                var attentionBuff = Unit.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_ATTENTION);
                if (attentionBuff?.IsValid() == true)
                    aggro += (int)attentionBuff.Value;
                else
                    Unit.RemoveConditionBuff(SkillEffectType.BUFF_STATE_ATTENTION);

                var sevenDevil = Unit.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_SEVEN_DEVIL);
                if (sevenDevil?.IsValid() == true)
                    aggro += (int)sevenDevil.Value;
                else
                    Unit.RemoveConditionBuff(SkillEffectType.BUFF_STATE_SEVEN_DEVIL);

                var eightDevil = Unit.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_EIGHT_DEVIL);
                if (eightDevil?.IsValid() == true)
                    aggro += (int)eightDevil.Value;
                else
                    Unit.RemoveConditionBuff(SkillEffectType.BUFF_STATE_EIGHT_DEVIL);

                var stealthBuff = Unit.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_STEALTH);
                if (stealthBuff?.IsValid() == true)
                    aggro -= (int)stealthBuff.Value;
                else
                    Unit.RemoveConditionBuff(SkillEffectType.BUFF_STATE_STEALTH);

                return Math.Max(AggroOrigin + GrowthAggro + aggro, 1);
            }
            set => AggroOrigin = value;
        }
        public int AttackDelay;
        public int FirstTakeDamage;
        public float AddDamageRateToHuman;
        public float AddDamageRateToBeast;
        public float AddDamageRateToMechanical;
        public float AddDamageRateToKimera;
        public float AddDamageRateToNormalAttack;
        public float ShieldDamageRate = 1;
        public float ShieldStart;
        public float ShieldReduceRate;
        public float ShieldGenerationRate;
    }
}
