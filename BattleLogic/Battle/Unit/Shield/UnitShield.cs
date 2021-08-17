using System;
using AkaData;
using AkaEnum;

namespace BattleLogic
{
    public class UnitShield
    {
        private int _shield;
        private float _maintainMilliSeconds;
        private UnitPassive _unitPassive;

        public int Shield
        {
            get
            {
                if (EndTime <= DateTime.UtcNow)
                    _shield = 0;

                return _shield;
            }
            set
            {
                _shield = value;
                _unitPassive?.PassiveConditionCheck(PassiveConditionType.PerShield, value);
            }
        }

        public uint CardId { get; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public double RemainMilliseconds => (EndTime - StartTime).TotalMilliseconds;
        public bool IsValid => Shield > 0;

        public UnitShield(uint cardId, DataSkillOption dataSkillOption, int bulletTime, UnitPassive passive)
        {
            Shield = (int)dataSkillOption.Value2;
            CardId = cardId;

            var nowDateTime = DateTime.UtcNow;
            StartTime = nowDateTime.AddMilliseconds(bulletTime);
            _maintainMilliSeconds = dataSkillOption.Value1 * 1000 + bulletTime;
            EndTime = nowDateTime.AddMilliseconds(_maintainMilliSeconds);

            _unitPassive = passive;
        }

        public void AddBulletTime(int milliseconds)
        {
            EndTime = EndTime.AddMilliseconds(milliseconds);
        }

        public int Decrease(float damage)
        {
            if (IsEnd())
                return (int)damage;

            var shield = Shield;
            Shield -= (int)damage;

            if (Shield <= 0)
                End();

            return (int)damage - shield;
        }

        public void End()
        {
            Shield = 0;
        }

        public bool IsEnd()
        {
            return Shield <= 0 || EndTime < DateTime.UtcNow;
        }
    }
}