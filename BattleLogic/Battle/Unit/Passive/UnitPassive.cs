using AkaData;
using AkaEnum;
using System.Collections.Generic;

namespace BattleLogic
{
    public class UnitPassive
    {
        List<Passive> _passives = new List<Passive>();
        private Unit _unit;


        public UnitPassive(Unit unit, DataWeaponStat dataWeaponStat, List<uint> affixList, List<uint> treasureIdList)
        {
            _unit = unit;
            UnitPassiveInitialize(dataWeaponStat, affixList, treasureIdList);
        }

        private void UnitPassiveInitialize(DataWeaponStat dataWeaponStat, List<uint> affixList, List<uint> treasureIdList)
        {
            if (UnitHasPassive())
            {
                for (var i = 0; i < _unit.UnitData.UnitIdentifier.PassiveConditionId.Count; i++)
                {
                    _passives.Add(PassiveFactory.CreatePassive(_unit, _unit.UnitData.UnitIdentifier.PassiveConditionId[i]));
                }
            }

            if (dataWeaponStat != null)
            {
                var passiveId = Data.GetWeaponStat(dataWeaponStat.WeaponId, dataWeaponStat.Level).PassiveConditionId;
                if (passiveId != 0)
                    _passives.Add(PassiveFactory.CreatePassive(_unit, passiveId));
            }

            if (affixList != null)
            {
                foreach (var affixId in affixList)
                {
                    var passiveId = Data.GetDataAffix(affixId).PassiveConditionId;
                    if (passiveId != 0)
                        _passives.Add(PassiveFactory.CreatePassive(_unit, passiveId));
                }
            }

            treasureIdList?.ForEach(treasureId =>
            {
                var passiveId = Data.GetTreasure(treasureId).PassiveConditionId;
                if (passiveId != 0)
                    _passives.Add(PassiveFactory.CreatePassive(_unit, passiveId));
            });
        }

        private bool UnitHasPassive()
        {
            return _unit.UnitData.UnitIdentifier.PassiveConditionId.Count > 0 && _unit.UnitData.UnitIdentifier.PassiveConditionId[0] != 0;
        }

        public void PassiveConditionCheck(PassiveConditionType passiveConditionType)
        {
            if (NoHasPassive())
                return;

            for (var i = _passives.Count - 1; i >= 0; i--)
            {
                if (_passives[i].IsPassiveExceedLimit())
                {
                    _passives.RemoveAt(i);
                    continue;
                }

                if (_passives[i].PassiveConditionCheck(passiveConditionType))
                {
                    if (_passives[i].PassiveType == PassiveType.Active)
                        _passives[i].EnqueueSkill();
                    else if (_passives[i].PassiveType == PassiveType.NonActive)
                        _passives[i].DoSkill();
                }

            }
        }

        public void PassiveConditionCheck(PassiveConditionType passiveConditionType, int baseValue, int compareValue)
        {
            if (NoHasPassive())
                return;

            for (var i = _passives.Count - 1; i >= 0; i--)
            {
                if (_passives[i].IsPassiveExceedLimit())
                {
                    _passives.RemoveAt(i);
                    continue;
                }

                if (_passives[i].PassiveConditionCheck(passiveConditionType, baseValue, compareValue))
                    _passives[i].EnqueueSkill();
            }
        }

        public void PassiveConditionCheck(PassiveConditionType passiveConditionType, float compareValue)
        {
            if (NoHasPassive())
                return;

            for (var i = _passives.Count - 1; i >= 0; i--)
            {
                if (_passives[i].IsPassiveExceedLimit())
                {
                    _passives.RemoveAt(i);
                    continue;
                }

                if (_passives[i].PassiveConditionCheck(passiveConditionType, compareValue))
                    _passives[i].EnqueueSkill();
            }
        }

        public bool NoHasPassive()
        {
            return _passives.Count == 0;
        }
    }
}
