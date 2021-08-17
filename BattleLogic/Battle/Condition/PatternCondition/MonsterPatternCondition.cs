using AkaData;
using AkaEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using AkaUtility;

namespace BattleLogic
{
    public abstract class MonsterPatternCondition
    {
        private static readonly Dictionary<OperationConditionType, Func<int, int, bool>> _predicator =
            new Dictionary<OperationConditionType, Func<int, int, bool>>(OperationConditionTypeComparer.Comparer)
            {
                { OperationConditionType.Over , (referenceVal, comparisonValue) => referenceVal < comparisonValue  },
                { OperationConditionType.Less , (referenceVal, comparisonValue) => referenceVal > comparisonValue  },
                { OperationConditionType.Equal , (referenceVal, comparisonValue) => referenceVal == comparisonValue  },
                { OperationConditionType.OverAndEqual , (referenceVal, comparisonValue) => referenceVal <= comparisonValue  },
                { OperationConditionType.LessAndEqual , (referenceVal, comparisonValue) => referenceVal >= comparisonValue  },
            };

        private static readonly Dictionary<ActionPatternType,Type> _conditionTypeOfassembly;
        public DataMonsterPatternCondition ConditionData { get; }

        protected int CurrentActionPatternValue { get; set; }
        protected int DidActionPatternValue { get; set; }
        protected uint MonsterID { get; }

        protected AkaEnum.Battle.PlayerType Player { get; }
        protected AkaEnum.Battle.PlayerType EnemyPlayer { get; }

        static MonsterPatternCondition()
        {

            _conditionTypeOfassembly = System.Reflection.Assembly.GetCallingAssembly()
                                      .GetTypes()
                                      .Where(type => type.IsSubclassOf(typeof(MonsterPatternCondition)) && type.GetCustomAttributes(typeof(PatternConditionAttribute), false).Length != 0)
                                      .ToDictionary(type => type.GetCustomAttributes(typeof(PatternConditionAttribute), false).Cast<PatternConditionAttribute>().First().PatternType
                                                   , type => type);
        }

        public static List<MonsterPatternCondition> FromData(uint unitId, IEnumerable<uint> conditionIds, AkaEnum.Battle.PlayerType player, AkaEnum.Battle.PlayerType enemyPlayer)
        {
            return conditionIds.Select<uint, MonsterPatternCondition>(conditionId =>
                   {
                       if (conditionId != 0)
                       {
                           var patternCondition = Data.GetMonsterPatternCondition(conditionId);
                           if (_conditionTypeOfassembly.TryGetValue(patternCondition?.ActionPatternType ?? ActionPatternType.None, out var conditionType))
                           {
                               return Activator.CreateInstance(conditionType, unitId, patternCondition, player, enemyPlayer) as MonsterPatternCondition;
                           }
                       }
                       return null;
                   })
                    .Where(condition => condition != null)
                    .ToList();
        }

        internal MonsterPatternCondition(uint unitId, DataMonsterPatternCondition origin, AkaEnum.Battle.PlayerType player, AkaEnum.Battle.PlayerType enemyPlayer)
        {
            MonsterID = unitId;
            ConditionData = origin;
            Player = player;
            EnemyPlayer = enemyPlayer;
            Reset();
        }


        bool Predicate(int refenectVal, int comparisionVal)
        {
            return _predicator[ConditionData.OperationConditionType](refenectVal, comparisionVal);
        }

        protected bool IsMe(AkaEnum.Battle.PlayerType player, uint unitId)
        {
            return IsMyTeam(player) && MonsterID == unitId;
        }

        protected bool IsMyTeam(AkaEnum.Battle.PlayerType player)
        {
            return Player == player;
        }

        public void Reset()
        {
            DidActionPatternValue = 0;
            CurrentActionPatternValue = 0;
        }
        public virtual bool Check()
        {
            return CurrentActionPatternValue != 0 && Predicate(ConditionData.ActionPatternValue, CurrentActionPatternValue - DidActionPatternValue);
        }

        public virtual void DidAction()
        {
            DidActionPatternValue = CurrentActionPatternValue;
        }

        public virtual void UpdateCondition()
        {
        }
        public virtual void UpdateCondition(AkaEnum.Battle.PlayerType player , uint monsterId)
        {
        }
        public virtual void UpdateCondition(AkaEnum.Battle.PlayerType player, uint unitId, DamageReasonType reasonType)
        {
        }
        public virtual void UpdateCondition(AkaEnum.Battle.PlayerType player, uint unitId, int maxValue, int currentValue)
        {
        }

        public virtual void UpdateCondition(AkaEnum.Battle.PlayerType player, uint unitId, int stack)
        {
        }
    }
}
