using AkaData;
using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System;

namespace Common.Entities.Battle
{
    public class VirtualRankUnit
    {
        private DBContext _db;
        private uint  _userId;
        private byte _deckNum;
        private ModeType _modeType;
        private ProtoOnBattleResult _protoOnBattleResult;
        private UnitsVirtualRankChangeData _unitsRankChangeData = new UnitsVirtualRankChangeData();

        private Dictionary<uint, ProtoRankData> _unitsDbData = new Dictionary<uint, ProtoRankData>();

        public VirtualRankUnit(DBContext db, uint userId, byte deckNum, ModeType modeType, ProtoOnBattleResult protoOnBattleResult)
        {
            _db = db;
            _userId = userId;
            _deckNum = deckNum;
            _modeType = modeType;
            _protoOnBattleResult = protoOnBattleResult;
        }

        public async Task<int> GetSumOfUnits()
        {
            await GetUnitsDbData();

            var sum = _unitsDbData.Values.Sum(rankData => rankData.CurrentSeasonRankPoint);
            return sum;
        }
               
        public async Task<UnitsVirtualRankChangeData> ApplyRankForWinAndLose(BattleResultType battleResultType)
        {
            await GetUnitsDbData();

            foreach (var unit in _unitsDbData)
            {
                ProtoRankData newUnitData;
                if (battleResultType == BattleResultType.Win)
                    newUnitData = GetNewRankUnitForWin(unit.Value);
                else
                    newUnitData = GetNewRankUnitForLose(unit.Value);

                await UpdateUnitRankInfo(newUnitData, unit.Key);
                SetUnitsChangeData(newUnitData, unit);
            }

            return _unitsRankChangeData;
        }
        
        public async Task<Dictionary<uint, ProtoRankData>> ApplyRankForDraw()
        {
            await GetUnitsDbData();
            Dictionary<uint, ProtoRankData> UnitsRankData = new Dictionary<uint, ProtoRankData>();
            foreach (var unit in _unitsDbData)
            {
                UnitsRankData.Add(unit.Key, new ProtoRankData { CurrentSeasonRankPoint = unit.Value.CurrentSeasonRankPoint });
            }
            return UnitsRankData;
        }
        

        private  ProtoRankData GetNewRankUnitForWin(ProtoRankData currentRankData)
        {
            var newCurrentRankLevel = GetNextCurrentRankLevelForWin(currentRankData);
            var newRankPoint = GetNewRankPointForWin(currentRankData);

            return new ProtoRankData
            {
                CurrentRankLevel = newCurrentRankLevel,
                MaxRankLevel = Math.Max(newCurrentRankLevel, currentRankData.MaxRankLevel),
                CurrentSeasonRankPoint = newRankPoint,
            };
        }

        private ProtoRankData GetNewRankUnitForLose(ProtoRankData currentRankData)
        {
            var newCurrentRankLevel = GetNextCurrentRankLevelForLose(currentRankData);
            var newRankPoint = GetNewRankPointForLose(currentRankData);

            return new ProtoRankData
            {
                CurrentRankLevel = newCurrentRankLevel,
                MaxRankLevel = currentRankData.MaxRankLevel,
                CurrentSeasonRankPoint = newRankPoint,
            };
        }

        private uint GetNextCurrentRankLevelForWin(ProtoRankData currentRankData)
        {
            uint nextCurrentRankLevel = currentRankData.CurrentRankLevel;
            if ((int)Data.GetConstant(DataConstantType.MAX_UNIT_RANK).Value != nextCurrentRankLevel)
            {
                if (IsFullRankPointForRankLevelUp(currentRankData))
                {
                    nextCurrentRankLevel += 1;
                }
                    
            }
            return nextCurrentRankLevel;

        }

        private uint GetNextCurrentRankLevelForLose(ProtoRankData currentRankData)
        {
            uint nextRankLevel = currentRankData.CurrentRankLevel;
            if (currentRankData.MaxRankLevel != 1)
            {
                if (IsRankPointUnderCurrentRankLevel(currentRankData))
                    nextRankLevel -= 1;
            }
            return nextRankLevel;
        }

        private bool IsRankPointUnderCurrentRankLevel(ProtoRankData currentRankData)
        {
            if (currentRankData.CurrentRankLevel == 1)
                return false;

            return Data.GetUnitRankPoint(currentRankData.CurrentRankLevel).LosePoint + currentRankData.CurrentSeasonRankPoint <
                Data.GetUnitRankPoint(currentRankData.CurrentRankLevel - 1).NeedRankPointForNextLevelUp;
        }

        private int GetNewRankPointForWin(ProtoRankData rankData)
        {
            return rankData.CurrentSeasonRankPoint + Data.GetUnitRankPoint((uint)rankData.CurrentRankLevel).WinPoint;
        }

        private int GetNewRankPointForLose(ProtoRankData rankData)
        {
            var nextPoint = rankData.CurrentSeasonRankPoint + Data.GetUnitRankPoint((uint)rankData.CurrentRankLevel).LosePoint;
            return nextPoint < 0 ? 0 : nextPoint;
        }
      
        private bool IsFullRankPointForRankLevelUp(ProtoRankData currentRankData)
        {
            return Data.GetUnitRankPoint((uint)currentRankData.CurrentRankLevel).WinPoint + currentRankData.CurrentSeasonRankPoint >=
                Data.GetUnitRankPoint((uint)currentRankData.CurrentRankLevel).NeedRankPointForNextLevelUp;
        }

        private async Task GetUnitsDbData()
        {
            if (_unitsDbData.Count != 0)
                return;

            var strSlotType = ((int)SlotType.Unit).ToString();
            var strModeType = ((int)_modeType).ToString();

            var query = new StringBuilder();
            query.Append("SELECT a.id, a.maxVirtualRankLevel, a.currentVirtualRankLevel, a.currentVirtualRankPoint "
                        +"FROM units a LEFT OUTER JOIN decks b ON b.userId = a.userId ")
                .Append("AND b.modeType = ").Append(strModeType)
                .Append(" AND b.deckNum = ").Append(_deckNum.ToString())
                .Append(" AND b.slotType = ").Append(strSlotType)
                .Append(" WHERE a.userId = ").Append(_userId.ToString()).Append(" AND a.id = b.classId;");

            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                while (cursor.Read())
                {
                    _unitsDbData.Add((uint)cursor["id"], new ProtoRankData
                    {
                        MaxRankLevel = (uint)cursor["maxVirtualRankLevel"],
                        CurrentRankLevel = (uint)cursor["currentVirtualRankLevel"],
                        CurrentSeasonRankPoint = cursor.GetInt32(3),
                    });
                }
            }
        }

        private async Task UpdateUnitRankInfo(ProtoRankData newUnitData, uint id)
        {
            var query = new StringBuilder();
            query.Append("UPDATE units SET  maxVirtualRankLevel=").Append(newUnitData.MaxRankLevel.ToString())
                .Append(", currentVirtualRankLevel =").Append(newUnitData.CurrentRankLevel.ToString())
                .Append(", currentVirtualRankPoint=").Append(newUnitData.CurrentSeasonRankPoint.ToString())
                .Append(" WHERE userId=").Append(_userId.ToString()).Append(" AND id=").Append(id).Append(";");
            await _db.ExecuteNonQueryAsync(query.ToString());
        }

        private void SetUnitsChangeData(ProtoRankData newUnitData, KeyValuePair<uint, ProtoRankData> unit)
        {
            var changePoint = newUnitData.CurrentSeasonRankPoint - unit.Value.CurrentSeasonRankPoint;
            _unitsRankChangeData.ChangePoint += changePoint;
            

            _unitsRankChangeData.UnitsRankData.Add(unit.Key, newUnitData);
            _unitsRankChangeData.UnitsChangePoints.Add(unit.Key, changePoint);
        }
    }
}
