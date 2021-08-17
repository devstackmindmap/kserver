

using AkaData;
using System.Collections.Generic;
using System.Linq;
using AkaUtility;

namespace BattleLogic
{
    public class Pattern
    {
        public uint PatternId;
        public uint CardStatId;
        public uint UnitId;
        public bool Deactived;

        //MonsterPatternFlowIdList;
        public IEnumerable<MonsterPatternCondition> CheckConditions;
        public List<MonsterPatternCondition> Conditions;
        public List<MonsterPatternCondition> ActiveConditions;
        public List<MonsterPatternCondition> DeactiveConditions;
        private bool _isDoingAction = false;
        private int _didActionCount = 0;
        public int _maxRepeatCount;


        //public List<FlowPattern> flowPatterns;

        public Pattern(int maxRepeatcount)
        {
            _maxRepeatCount = maxRepeatcount;
        }

        public void ResetConditions()
        {
            _didActionCount = 0;
            Conditions.ForEach(condition => condition.Reset());
        }

        public bool HasActiveCondition()
        {
            return ActiveConditions.FirstOrDefault()?.GetType() != typeof(BoolPatternCondition) || DeactiveConditions.First().GetType() != typeof(BoolPatternCondition);
        }

        public virtual bool CanDoAction()
        {
            if (Deactived == true)
            {
                Deactived = false == ActiveConditionsCheck;
                if (Deactived == false)
                {
                    ResetConditions();
                    ActiveConditions.Clear();
                }
            }

            if (Deactived == false)
            {
                Deactived = DeactiveConditionsCheck;
            }

            if (_isDoingAction)
                return false;

            if (Deactived)
                return false;

            if (_maxRepeatCount - _didActionCount <= 0)
                return false;

            if (Conditions.Any(condition => condition.Check()) == false)
                return false;

            return true;
        }

        internal virtual bool ActiveConditionsCheck => ActiveConditions.Any(condition => condition.Check());
        internal virtual bool DeactiveConditionsCheck => DeactiveConditions.Any(condition => condition.Check());

        internal virtual void DoAction()
        {
            _isDoingAction = true;
        }
        internal virtual void DidAction()
        {
            _didActionCount++;
            Conditions.ForEach(condition => condition.DidAction());
            _isDoingAction = false;
        }
    }
}
