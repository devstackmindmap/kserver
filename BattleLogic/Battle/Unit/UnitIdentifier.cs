using System.Collections.Generic;
using AkaData;
using AkaEnum;

namespace BattleLogic
{
    public class UnitIdentifier
    {
        public int UnitPositionIndex;
        public uint UnitId;
        public uint Level;
        public UnitType UnitType;
        public List<uint> PassiveConditionId;
        public string UnitInitial;
        public MonsterType MonsterType;

        public UnitIdentifier(uint unitId, uint level, int position, uint skinId, List<uint> passiveConditionId, 
            MonsterType monsterType = AkaEnum.MonsterType.Normal )
        {
            var dataUnit = Data.GetUnit(unitId);

            UnitPositionIndex = position;
            UnitId = unitId;
            Level = level;
            UnitType = dataUnit.UnitType;
            PassiveConditionId = passiveConditionId;
            MonsterType = monsterType;

            if (skinId == 0)
            {
                UnitInitial = dataUnit.UnitInitial + "_Basic";
            }
            else
            {
                var skinData = Data.GetDataSkin(skinId);
                UnitInitial = skinData.SheetName;
            }
        }
    }
}
