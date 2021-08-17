using AkaUtility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleLogic
{
    public class Interaction
    {
        protected Unit _performer;

        public Interaction(Unit performer)
        {
            _performer = performer;
        }

        protected Unit GetUnitByAggro(Dictionary<int, Unit> units)
        {
            var aggroSum = units.Sum(data => data.Value.UnitData.UnitStatus.Aggro);
            var choiceValue = AkaRandom.Random.Next(0, aggroSum);

            var currentSum = 0;
            foreach (var value in units)
            {
                currentSum += value.Value.UnitData.UnitStatus.Aggro;
                if (choiceValue < currentSum)
                    return value.Value;
            }

            throw new Exception("GetUnitByAggro Func Logic is wrong");
        }
    }
}
