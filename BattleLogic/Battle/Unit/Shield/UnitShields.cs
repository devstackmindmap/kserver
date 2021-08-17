using System.Collections.Generic;
using AkaData;
using AkaEnum;
using CommonProtocol;

namespace BattleLogic
{
    public class UnitShields
    {
        private readonly List<UnitShield> _shields = new List<UnitShield>();
        private readonly UnitShieldTimers _timers = new UnitShieldTimers();

        public int Shield
        {
            get
            {
                var shield = 0;

                for (var i = 0; i < _shields.Count; i++)
                {
                    shield += _shields[i].Shield;
                }

                return shield;
            }
        }

        public UnitShield AddShield(Card card, DataSkillOption dataSkillOption, int bulletTime, UnitPassive unitPassive)
        {
            RemoveContainShield(card.CardId);
            var addShield = new UnitShield(card.CardId, dataSkillOption, bulletTime, unitPassive);
            _shields.Add(addShield);

            _shields.Sort((a, b) => a.RemainMilliseconds.CompareTo(b.RemainMilliseconds));
            return addShield;
        }

        private void RemoveContainShield(uint cardId)
        {
            for (var i = _shields.Count - 1; i >= 0; i--)
            {
                if (_shields[i].CardId == cardId)
                {
                    _timers.Remove(cardId);
                    _shields.RemoveAt(i);
                }
            }
        }

        public void AddBulletTime(int bulletTime)
        {
            for (var i = 0; i < _shields.Count; i++)
            {
                if (_shields[i].IsValid == false)
                    continue;

                _shields[i].AddBulletTime(bulletTime);
            }
        }

        public (float ExtraDamage, float realDamage) GetExtraDamageAndDoDamageToShield(float damage, float shieldDamageRate)
        {
            damage = GetDamageApplyShieldDamageRate(damage, shieldDamageRate);
            if (Shield <= 0)
                return (damage, damage);

            var shield = Shield - (int)damage;
            if (shield < 0)
            {
                var extraDamage = shield * -1;
                ShieldEnd();
                return (extraDamage, damage);
            }

            var decreaseDamage = damage;
            for (var i = 0; i < _shields.Count; i++)
            {
                if (decreaseDamage <= 0)
                    break;

                decreaseDamage = _shields[i].Decrease(decreaseDamage);
            }

            CheckShieldEnd();

            return (0, damage);
        }

        private float GetDamageApplyShieldDamageRate(float damage, float shieldDamageRate)
        {
            if (Shield > 0)
                return damage * shieldDamageRate;

            return damage;
        }

        public void ShieldEnd()
        {
            for (var i = 0; i < _shields.Count; i++)
            {
                _timers.Remove(_shields[i].CardId);
                _shields[i].End();
            }

            _shields.Clear();
        }

        private void CheckShieldEnd()
        {
            for (var i = _shields.Count - 1; i >= 0; i--)
            {
                if (_shields[i].IsEnd())
                    _shields.RemoveAt(i);
            }
        }

        public void Dispose()
        {
            _timers.Dispose();
        }

        public void Pause()
        {
            _timers.Pause();
        }

        public void Restart(int bulletTime)
        {
            _timers.Restart(bulletTime);
        }

        public List<ProtoShieldInfo> GetShieldInfos()
        {
            CheckShieldEnd();

            var shieldInfos = new List<ProtoShieldInfo>();

            for (var i = 0; i < _shields.Count; i++)
            {
                var shield = _shields[i].Shield;
                if (shield <= 0)
                    continue;

                shieldInfos.Add(new ProtoShieldInfo()
                {
                    CardId = _shields[i].CardId,
                    StartTime = _shields[i].StartTime.Ticks,
                    EndTime = _shields[i].EndTime.Ticks,
                    Shield = shield
                });
            }

            return shieldInfos;
        }
    }
}