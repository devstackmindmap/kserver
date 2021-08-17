using AkaData;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleLogic
{
    public class UnitBuffs : IDisposable
    {
        private readonly Dictionary<SkillEffectType, IBuffSkill> _buffs = new Dictionary<SkillEffectType, IBuffSkill>(SkillEffectTypeComparer.Comparer);
        private readonly UnitBuffTimers _unitBuffTimers = new UnitBuffTimers();

        public Dictionary<SkillEffectType, IBuffSkill> Buffs { get { return _buffs; } }


        public void AddBulletTime(int bulletTime)
        {
            foreach (var buff in _buffs)
            {
                if (buff.Value.IsValid() == false)
                    continue;

                buff.Value.AddBulletTime(bulletTime);
            }
        }

        public BaseSkillProto AddBuff(SkillEffectType skillEffectType, DataSkillOption skillOption, Unit performer, Unit target, int bulletTime)
        {
            if (IsContainBuffAndRemoveBuff(skillEffectType))
            {
                if (skillEffectType == SkillEffectType.BUFF_STATE_POISON)
                {
                    _buffs[skillEffectType].DoSkill(skillOption, performer, target, bulletTime);
                }
                else
                {
                    _buffs[skillEffectType].UpdateSkillOption(skillOption);
                    _buffs[skillEffectType].AddEndTime((int)(skillOption.Value1 * 1000));
                    _buffs[skillEffectType].UpdateBuffStartTime();
                    // EndDateTime에 BulletTime 빼주는 이유는 ProgressSkill에서 Restart할때 BulletTime만큼 Timer에 더해주고 있기 때문.
                    _unitBuffTimers.ReInterval(skillEffectType, _buffs[skillEffectType].EndDateTime.AddMilliseconds(-bulletTime));
                }
            }
            else
            {
                _buffs.Add(skillEffectType, SkillFactory.CreateConditionBuffSkill(skillEffectType));
                _buffs[skillEffectType].DoSkill(skillOption, performer, target, bulletTime);
            }

            if (skillEffectType == SkillEffectType.BUFF_STATE_IGNITION)
                target.BattleHelper.BattlePatternBehavior.DoHasIgnitionPatternSchedule(target.PlayerType, target.UnitData.UnitIdentifier.UnitId, (int)_buffs[skillEffectType].Value);

            return new ProtoCommonBuffState()
            {
                SkillEffectType = SkillEffectType.COMMON_BUFF_STATES,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId,
                StartTime = _buffs[skillEffectType].BuffStartTime.Ticks,
                EndTime = _buffs[skillEffectType].EndDateTime.Ticks
            };
        }

        private bool IsContainBuffAndRemoveBuff(SkillEffectType skillEffectType)
        {
            if (_buffs.ContainsKey(skillEffectType))
            {
                if (_buffs[skillEffectType].IsValid())
                    return true;
                else
                    RemoveBuff(skillEffectType);
            }
            return false;
        }

        public bool RemoveBuff(SkillEffectType skillEffectType)
        {
            if (_buffs.ContainsKey(skillEffectType) == false)
                return false;

            AkaLogger.Logger.Instance().Debug($"EndDatetime:{_buffs[skillEffectType].EndDateTime}");
            _unitBuffTimers.RemoveBuff(skillEffectType);
            _buffs[skillEffectType].BuffEnd();
            _buffs.Remove(skillEffectType);
            return true;
        }

        public List<SkillEffectType> RemoveBuffs()
        {
            var removedTypes = new List<SkillEffectType>();
            foreach (var buff in _buffs)
            {
                removedTypes.Add(buff.Key);
                buff.Value.BuffEnd();
            }

            _buffs.Clear();
            _unitBuffTimers.RemoveBuffs();

            return removedTypes;
        }

        public List<SkillEffectType> RemoveBuffs(List<SkillEffectType> ignoreTypes, params StateGoodBadType[] goodBadTypes)
        {
            var removedTypes = new List<SkillEffectType>();
            foreach (var buff in _buffs)
            {
                var goodBadType = Data.GetState(buff.Value.SkillEffectType).GoodBad;
                if (goodBadTypes.Contains(goodBadType) == false)
                    continue;

                if (ignoreTypes.Contains(buff.Key))
                    continue;

                removedTypes.Add(buff.Key);
                buff.Value.BuffEnd();
            }

            for (var i = 0; i < removedTypes.Count; i++)
            {
                _buffs.Remove(removedTypes[i]);
            }

            _unitBuffTimers.RemoveBuffs(removedTypes);

            return removedTypes;
        }

        public void AddTimer(SkillEffectType skillEffectType, float interval, Action action)
        {
            _unitBuffTimers.AddBuff(skillEffectType, interval, action);
        }

        public void Dispose()
        {
            _unitBuffTimers.Dispose();
        }

        public void Pause()
        {
            _unitBuffTimers.Pause();
        }

        public void Restart(int bulletTime)
        {
            _unitBuffTimers.Restart(bulletTime);
        }

        public IBuffSkill GetBuffSkill(SkillEffectType skillEffectType)
        {
            if (IsContainBuffSkill(skillEffectType) == false)
                return null;

            return _buffs[skillEffectType];
        }

        public bool IsContainBuffSkill(SkillEffectType skillEffectType)
        {
            return _buffs.ContainsKey(skillEffectType);
        }

        public List<ProtoCurrentBuffInfo> IncreaseConditionTime(float increaseTime)
        {
            var buffs = new List<ProtoCurrentBuffInfo>();
            var increaseMilli = (int)(increaseTime * 1000);

            foreach (var buff in _buffs)
            {
                buff.Value.AddEndTime(increaseMilli);
                _unitBuffTimers.ReInterval(buff.Key, buff.Value.EndDateTime);

                buffs.Add(new ProtoCurrentBuffInfo
                {
                    SkillOptionId = buff.Value.SkillOptionId,
                    BuffStartTime = buff.Value.BuffStartTime.Ticks,
                    BuffEndTime = buff.Value.EndDateTime.Ticks,
                    RemainCount = buff.Value.RemainCount
                });
            }

            return buffs;
        }

        public List<ProtoCurrentBuffInfo> IncreaseConditionTimeRate(float rate)
        {
            var buffs = new List<ProtoCurrentBuffInfo>();

            foreach (var buff in _buffs)
            {
                var remainTime = (buff.Value.EndDateTime - DateTime.UtcNow).TotalMilliseconds;
                var addTime = remainTime * rate - remainTime;
                buff.Value.AddEndTime((int)addTime);
                _unitBuffTimers.ReInterval(buff.Key, buff.Value.EndDateTime);

                buffs.Add(new ProtoCurrentBuffInfo
                {
                    SkillOptionId = buff.Value.SkillOptionId,
                    BuffStartTime = buff.Value.BuffStartTime.Ticks,
                    BuffEndTime = buff.Value.EndDateTime.Ticks,
                    RemainCount = buff.Value.RemainCount
                });
            }

            return buffs;
        }
    }
}
