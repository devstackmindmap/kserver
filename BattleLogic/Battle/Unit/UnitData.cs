using System;
using System.Collections.Generic;
using System.Text;

namespace BattleLogic
{
    public class UnitData
    {
        public UnitStatus UnitStatus;
        public UnitIdentifier UnitIdentifier;

        public UnitData(UnitStatus unitStatus, UnitIdentifier unitIdentifier)
        {
            UnitStatus = unitStatus;
            UnitIdentifier = unitIdentifier;
        }
    }
}
