using AkaData;
using AkaEnum;
using System;

namespace BattleLogic
{
    public abstract class BuffSkill : IBuffSkill
    {
        public SkillEffectType SkillEffectType { get; protected set; }
        protected DateTime _endDateTime;
        protected float _maintainMilliSeconds;
        protected Unit _unit;

        public DateTime EndDateTime => _endDateTime;

        public virtual bool IsValid(double delayMilliseconds = 0d) { return _endDateTime > DateTime.UtcNow.AddMilliseconds(delayMilliseconds); }

        public abstract float Value { get; }

        public uint SkillOptionId { get; private set; }

        public DateTime BuffStartTime { get; private set; }

        public virtual int RemainCount => 0;

        public void AddBulletTime(int milliseconds)
        {
            var logPrevEndTime = _endDateTime;

            _endDateTime = _endDateTime.AddMilliseconds(milliseconds);
            //AkaLogger.Logger.Instance().Info($"[Buff AddBulletTime] Type:{SkillEffectType}, Bullet:{milliseconds}, EndTime:{_endDateTime}:{_endDateTime.Millisecond}");

            //AkaLogger.Log.Battle.BuffEndTime.Log(_unit.MyPlayer.Battle.RoomId, _unit.UnitData.UnitIdentifier.UnitId, SkillEffectType, milliseconds, logPrevEndTime, _endDateTime, "AddBulletTime");
        }

        public virtual void AddEndTime(int milliseconds)
        {
            var logPrevEndTime = _endDateTime;
            _endDateTime = _endDateTime.AddMilliseconds(milliseconds);
            //AkaLogger.Logger.Instance().Info($"[Buff AddEndTime] Type:{SkillEffectType}, Bullet:{milliseconds}, EndTime:{_endDateTime}:{_endDateTime.Millisecond}");
            //AkaLogger.Log.Battle.BuffEndTime.Log(_unit.MyPlayer.Battle.RoomId, _unit.UnitData.UnitIdentifier.UnitId, SkillEffectType, milliseconds, logPrevEndTime, _endDateTime, "AddEndTime");
        }

        public virtual void DoSkill(DataSkillOption option, Unit performer, Unit target, int bulletTime)
        {
            SkillOptionId = option.SkillOptionId;
            var nowDateTime = DateTime.UtcNow;
            BuffStartTime = nowDateTime.AddMilliseconds(bulletTime);
            SkillEffectType = option.SkillEffectType;
            _maintainMilliSeconds = (option.Value1 * 1000) + bulletTime;
            _endDateTime = nowDateTime.AddMilliseconds(_maintainMilliSeconds);
            _unit = target;
        }

        public void UpdateBuffStartTime()
        {
            BuffStartTime = DateTime.UtcNow;
        }

        public virtual void BuffEnd() { }

        public virtual void DecreaseCount() { }

        public virtual void UpdateSkillOption(DataSkillOption option) { }

        public virtual void MultipleStack(float rate) { }

        public abstract IBuffSkill Clone();
        public abstract void CalculateValue(ref float value, Unit target);
    }
}
