using AkaEnum;
using AkaEnum.Battle;
using AkaUtility;
using CommonProtocol;
using CommonProtocol.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BattleLogic
{
    

    class ReferenceInteger
    {
        private int _value;

        public void Add(int addValue)
        {
            Interlocked.Add(ref _value, addValue);
        }

        public void Set(int setValue)
        {
            Interlocked.Exchange(ref _value, setValue);
        }

        public void Increment()
        {
            Interlocked.Increment(ref _value);
        }

        public int Value => _value;
    }



    public class PlayerActionLog : ActionLog
    {
        public PlayerActionLog() : base(ActionStatusType.None, ActionStatusType.UnitAction) { }
    }

    public class UnitActionLog : ActionLog
    {
        public UnitActionLog() : base(ActionStatusType.UnitAction, ActionStatusType.CardAction) { }
    }

    public class SkillActionLog : ActionLog
    {
        public SkillActionLog() : base(ActionStatusType.CardAction, ActionStatusType.EndOfAction) { }
    }



    public class ActionLog 
    {
        private Dictionary<ActionStatusType, ReferenceInteger> _statusLog;
        
        protected ActionLog(ActionStatusType startStatus, ActionStatusType endStatus)
        {
            _statusLog = Enum.GetValues(typeof(ActionStatusType))
                             .OfType< ActionStatusType>()
                             .Where(actionStatusType => actionStatusType > startStatus && actionStatusType < endStatus )
                             .ToDictionary(enumValue => enumValue, _ => new ReferenceInteger());
        }

        public void AddStatus(ActionStatusType statusType, int value)
        {
            if (_statusLog.TryGetValue(statusType, out var storedValue))
            {
                storedValue.Add(value);
            }
        }

        public void IncreaseStatus(ActionStatusType statusType)
        {
            if (_statusLog.TryGetValue(statusType, out var storedValue))
            {
                storedValue.Increment();
            }
        }

        public void SetStatus(ActionStatusType statusType, int value)
        {
            if (_statusLog.TryGetValue(statusType, out var storedValue))
            {
                storedValue.Set(value);
            }
        }

        public List<ProtoActionStatus> ToResult(uint classId)
        {
            return _statusLog.Select(status => new ProtoActionStatus { ActionStatusType = status.Key, ClassId = classId, Value = status.Value.Value })
                             .ToList();
        }
    }
}
