using System;
using AkaEnum;

namespace BattleLogic
{
    public class PatternConditionAttribute : Attribute
    {
        public ActionPatternType PatternType;
    }
}
