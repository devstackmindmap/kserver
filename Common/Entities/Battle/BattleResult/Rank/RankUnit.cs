using AkaData;
using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System;
using AkaUtility;

namespace Common.Entities.Battle
{
    public class RankUnit
    {
        private DBContext _db;
        private uint _userId;

        private string _strDeckNum;
        private string _strModeType;
        private ProtoOnBattleResult _protoOnBattleResult;
        private UnitsRankChangeData _unitsRankChangeData = new UnitsRankChangeData();

        private Dictionary<uint, ProtoRankData> _unitsDbData = new Dictionary<uint, ProtoRankData>();

        private int _enemyTeamRankPoint;
        private double _rankPointWinRate;
        private double _rankPointLoseRate;
        private bool _isAi;

        public RankUnit(DBContext db, uint userId, string strDeckNum, int enemyTeamRankPoint, 
            string strModeType, ProtoOnBattleResult protoOnBattleResult, bool isAi)
        {
            _db = db;
            _userId = userId;
            _strDeckNum = strDeckNum;
            _strModeType = strModeType;
            _protoOnBattleResult = protoOnBattleResult;
            _enemyTeamRankPoint = enemyTeamRankPoint;
            _rankPointWinRate = 1;
            _rankPointLoseRate = 1;
            _isAi = isAi;
        }

        public async Task<int> GetSumOfUnits()
        {
            await GetUnitsDbData();

            var sum = _unitsDbData.Values.Sum(rankData => rankData.CurrentSeasonRankPoint);
            return sum;
        }

        public async Task<UnitsRankChangeData> ApplyRankForWinAndLose(BattleResultType battleResultType)
        {
            Dictionary<uint, ProtoRankData> enemyDbData = new Dictionary<uint, ProtoRankData>();
            var rankPoint = await GetSumOfUnits();
            SetUnderdogRating(rankPoint);

            foreach (var unit in _unitsDbData)
            {
                ProtoRankData newUnitData;
                if (battleResultType == BattleResultType.Win)
                    newUnitData = await GetNewRankUnitForWin(unit);
                else
                    newUnitData = await GetNewRankUnitForLose(unit);

                AkaLogger.Log.User.UnitRank.Log(_userId, unit.Key, newUnitData.CurrentRankLevel, newUnitData.CurrentSeasonRankPoint);

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
                UnitsRankData.Add(unit.Key, new ProtoRankData { MaxRankLevel = unit.Value.MaxRankLevel, CurrentSeasonRankPoint = unit.Value.CurrentSeasonRankPoint });
            }
            return UnitsRankData;
        }

        private void SetUnderdogRating(int myTeamRankPoint)
        {
            if (_enemyTeamRankPoint >= 0)
            {
                var rankInterval = Data.GetRankTierMatchingId(myTeamRankPoint) - Data.GetRankTierMatchingId(_enemyTeamRankPoint);
                var underData = Data.GetUnderdog(rankInterval);
                _rankPointWinRate = underData.WinRate;
                _rankPointLoseRate = underData.LoseRate;
            }
        }

        private async Task<ProtoRankData> GetNewRankUnitForWin(KeyValuePair<uint, ProtoRankData>  unit)
        {
            var newCurrentRankLevel = await GetNextCurrentRankLevelForWin(unit);
            var newRankPoint = GetNewRankPointForWin(unit.Value);

            return new ProtoRankData
            {
                CurrentRankLevel = newCurrentRankLevel,
                MaxRankLevel = Math.Max(newCurrentRankLevel, unit.Value.MaxRankLevel),
                CurrentSeasonRankPoint = newRankPoint,
                NextSeasonRankPoint = RankSeason.GetNextSeasonRankPoint(newRankPoint)
            };
        }

        private async Task<ProtoRankData> GetNewRankUnitForLose(KeyValuePair<uint, ProtoRankData> unit)
        {
            var newCurrentRankLevel = await GetNextCurrentRankLevelForLose(unit.Value);
            var newRankPoint = GetNewRankPointForLose(unit.Value);

            return new ProtoRankData
            {
                CurrentRankLevel = newCurrentRankLevel,
                MaxRankLevel = Math.Max(newCurrentRankLevel, unit.Value.MaxRankLevel),
                CurrentSeasonRankPoint = newRankPoint,
                NextSeasonRankPoint = RankSeason.GetNextSeasonRankPoint(newRankPoint)
            };
        }

        private async Task<uint> GetNextCurrentRankLevelForWin(KeyValuePair<uint, ProtoRankData> unit)
        {
            uint nextCurrentRankLevel = unit.Value.CurrentRankLevel;
            if ((int)Data.GetConstant(DataConstantType.MAX_UNIT_RANK).Value != nextCurrentRankLevel)
            {
                if (IsFullRankPointForRankLevelUp(unit.Value))
                {
                    nextCurrentRankLevel += 1;
                    if (nextCurrentRankLevel > unit.Value.MaxRankLevel)
                    {
                        await AddUnitRankUpReward(nextCurrentRankLevel);
                        AkaLogger.Log.User.UnitRank.Log(_userId, unit.Key, nextCurrentRankLevel);
                    }
                }  
            }
            return nextCurrentRankLevel;
        }

        private async Task<uint> GetNextCurrentRankLevelForLose(ProtoRankData currentRankData)
        {
            uint nextRankLevel = currentRankData.CurrentRankLevel;
            if (GetLosePoint(currentRankData.CurrentRankLevel) > 0)
            {
                if (IsFullRankPointForRankLevelUpForGetLossPoint(currentRankData))
                {
                    nextRankLevel += 1;
                    if (nextRankLevel > currentRankData.MaxRankLevel)
                        await AddUnitRankUpReward(nextRankLevel);
                }
            }
            else
            {
                if (currentRankData.MaxRankLevel != 1)
                {
                    if (IsRankPointUnderCurrentRankLevel(currentRankData))
                        nextRankLevel -= 1;
                }
            }
            return nextRankLevel;
        }

        private bool IsRankPointUnderCurrentRankLevel(ProtoRankData currentRankData)
        {
            if (currentRankData.CurrentRankLevel == 1)
                return false;

            return Math.Round(Double.Epsilon + _rankPointLoseRate * GetLosePoint(currentRankData.CurrentRankLevel)) 
                + currentRankData.CurrentSeasonRankPoint <
                Data.GetUnitRankPoint(currentRankData.CurrentRankLevel - 1).NeedRankPointForNextLevelUp;
        }

        private int GetNewRankPointForWin(ProtoRankData rankData)
        {
            return rankData.CurrentSeasonRankPoint 
                + (int)Math.Round(Double.Epsilon + _rankPointWinRate * GetWinPoint(rankData.CurrentRankLevel));
        }

        private int GetNewRankPointForLose(ProtoRankData rankData)
        {
            var nextPoint = rankData.CurrentSeasonRankPoint 
                + (int)Math.Round(Double.Epsilon +  _rankPointLoseRate * GetLosePoint(rankData.CurrentRankLevel));
            return nextPoint < 0 ? 0 : nextPoint;
        }
      
        private bool IsFullRankPointForRankLevelUp(ProtoRankData currentRankData)
        {
            return (int)Math.Round(Double.Epsilon + _rankPointWinRate * GetWinPoint(currentRankData.CurrentRankLevel)) 
                + currentRankData.CurrentSeasonRankPoint >= 
                Data.GetUnitRankPoint((uint)currentRankData.CurrentRankLevel).NeedRankPointForNextLevelUp;
        }

        private bool IsFullRankPointForRankLevelUpForGetLossPoint(ProtoRankData currentRankData)
        {
            return (int)Math.Round(Double.Epsilon + _rankPointWinRate * GetLosePoint(currentRankData.CurrentRankLevel)) 
                + currentRankData.CurrentSeasonRankPoint >=
                Data.GetUnitRankPoint((uint)currentRankData.CurrentRankLevel).NeedRankPointForNextLevelUp;
        }

        private async Task AddUnitRankUpReward(uint level)
        {
            var unitRank = Data.GetUnitRankPoint(level);
            var rewards = await Reward.Reward.GetRewards(_db, _userId, unitRank.RewardId, "UnitRankUpReward");
            if (null == _protoOnBattleResult.ItemResults)
                _protoOnBattleResult.ItemResults = new Dictionary<RewardCategoryType, List<ProtoItemResult>>(RewardCategoryTypeComparer.Comparer);

            if (_protoOnBattleResult.ItemResults.ContainsKey(RewardCategoryType.UnitRankUp))
                _protoOnBattleResult.ItemResults[RewardCategoryType.UnitRankUp].AddRange(rewards);
            else
                _protoOnBattleResult.ItemResults.Add(RewardCategoryType.UnitRankUp, rewards);
        }

        private async Task GetUnitsDbData(Dictionary<uint, ProtoRankData> unitDbData, string deckNum, string userId)
        {
            if (unitDbData.Count != 0)
                return;

            var strSlotType = ((int)SlotType.Unit).ToString();

            var query = new StringBuilder();
            query.Append("SELECT a.id, a.maxRankLevel, a.currentRankLevel, a.currentSeasonRankPoint, a.nextSeasonRankPoint "
                        + "FROM units a LEFT OUTER JOIN decks b ON b.userId = a.userId ")
                .Append("AND b.modeType = ").Append(_strModeType)
                .Append(" AND b.deckNum = ").Append(deckNum)
                .Append(" AND b.slotType = ").Append(strSlotType)
                .Append(" WHERE a.userId = ").Append(userId).Append(" AND a.id = b.classId;");

            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                while (cursor.Read())
                {
                    unitDbData.Add((uint)cursor["id"], new ProtoRankData
                    {
                        MaxRankLevel = (uint)cursor["maxRankLevel"],
                        CurrentRankLevel = (uint)cursor["currentRankLevel"],
                        CurrentSeasonRankPoint = cursor.GetInt32(3),
                        NextSeasonRankPoint = cursor.GetInt32(4)
                    });
                }
            }
        }

        private async Task GetUnitsDbData()
        {
            await GetUnitsDbData(_unitsDbData, _strDeckNum, _userId.ToString());
        }

        private async Task UpdateUnitRankInfo(ProtoRankData newUnitData, uint id)
        {
            var query = new StringBuilder();
            query.Append("UPDATE units SET maxRankLevel=").Append(newUnitData.MaxRankLevel)
                .Append(", currentRankLevel=").Append(newUnitData.CurrentRankLevel)
                .Append(", currentSeasonRankPoint=").Append(newUnitData.CurrentSeasonRankPoint)
                .Append(", nextSeasonRankPoint=").Append(newUnitData.NextSeasonRankPoint)
                .Append(" WHERE userId=").Append(_userId).Append(" AND id=").Append(id).Append(";");
            await _db.ExecuteNonQueryAsync(query.ToString());
        }

        private void SetUnitsChangeData(ProtoRankData newUnitData, KeyValuePair<uint, ProtoRankData> unit)
        {
            var changePoint = newUnitData.CurrentSeasonRankPoint - unit.Value.CurrentSeasonRankPoint;
            _unitsRankChangeData.ChangePoint += changePoint;
            
            var nextSeasonChangePoint = newUnitData.NextSeasonRankPoint - unit.Value.NextSeasonRankPoint;
            _unitsRankChangeData.NextSeasonChangePoint += nextSeasonChangePoint;

            _unitsRankChangeData.UnitsRankData.Add(unit.Key, newUnitData);
            _unitsRankChangeData.UnitsChangePoints.Add(unit.Key, (changePoint, nextSeasonChangePoint));
        }


        private int GetWinPoint(uint rankLevel)
        {
            if (_isAi)
                return Data.GetUnitRankPoint(rankLevel).AiWinPoint;
            else
                return Data.GetUnitRankPoint(rankLevel).WinPoint;
        }

        private int GetLosePoint(uint rankLevel)
        {
            if (_isAi)
                return Data.GetUnitRankPoint(rankLevel).AiLosePoint;
            else
                return Data.GetUnitRankPoint(rankLevel).LosePoint;
        }
    }
}
