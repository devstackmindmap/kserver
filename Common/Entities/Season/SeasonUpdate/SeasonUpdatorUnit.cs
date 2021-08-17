using AkaDB.MySql;
using AkaRedisLogic;
using Common.Entities.Battle;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Entities.Season
{
    public class SeasonUpdatorUnit
    {
        private DBContext _userDb;
        private uint _userId;
        private int _newUserCurrentRankData = 0;
        private int _newUserNextRankData = 0;
        private IDatabase _redis;
        private uint _serverSeason;
        private string _countryCode;

        public SeasonUpdatorUnit(DBContext userDb, IDatabase redis, uint userId, uint serverSeason, string countryCode)
        {
            _userDb = userDb;
            _userId = userId;
            _serverSeason = serverSeason;
            _redis = redis;
            _countryCode = countryCode;
        }

        public async Task<(int newUserCurrentRankData, int newUserNextRankData)> SeasonUpdate()
        {
            var query = new StringBuilder();
            query.Append("SELECT id, currentRankLevel, currentSeasonRankPoint, nextSeasonRankPoint FROM units WHERE userId =").Append(_userId).Append(";");

            List<(uint id, uint currentRankLevel, int newCurrentSeasonRankPoint, int newNextSeasonRankPoint)> newRankPoints
                = new List<(uint id, uint currentRankLevel, int newCurrentSeasonRankPoint, int newNextSeasonRankPoint)>();
            using (var cursor = await _userDb.ExecuteReaderAsync(query.ToString()))
            {
                while (cursor.Read())
                {
                    var newCurrentSeasonRankPoint = (int)cursor["nextSeasonRankPoint"];
                    var newNextSeasonRankPoint = RankSeason.GetNextSeasonRankPoint(newCurrentSeasonRankPoint);
                    newRankPoints.Add(((uint)cursor["id"], (uint)cursor["currentRankLevel"], newCurrentSeasonRankPoint, newNextSeasonRankPoint));
                }
            }

            foreach (var newRankPoint in newRankPoints)
            {
                await SetNewSeasonPoints(newRankPoint);
                await SeasonRedisUpdate(newRankPoint);
                _newUserCurrentRankData += newRankPoint.newCurrentSeasonRankPoint;
                _newUserNextRankData += newRankPoint.newNextSeasonRankPoint;
            }

            return (_newUserCurrentRankData, _newUserNextRankData);
        }

        private async Task SeasonRedisUpdate((uint unitId, uint currentRankLevel, int newCurrentSeasonRankPoint, int newNextSeasonRankPoint) newRankPoint)
        {
            if (newRankPoint.newNextSeasonRankPoint > 0)
            {
                await GameBattleRankRedisJob.SetRankKnightLeagueUnitAsync(_redis, _userId, newRankPoint.unitId, newRankPoint.newNextSeasonRankPoint, _serverSeason + 1);
                await GameBattleRankRedisJob.SetRankKnightLeagueUnitCountryAsync(_redis, _userId, newRankPoint.unitId, newRankPoint.newNextSeasonRankPoint, _serverSeason + 1, _countryCode);
            }
        }

        private async Task SetNewSeasonPoints((uint unitId, uint currentRankLevel, int newCurrentSeasonRankPoint, int newNextSeasonRankPoint) newRankPoint)
        {
            var newCurrentRankLevel = RankSeason.GetUnitNewCurrentLevel(newRankPoint.currentRankLevel, newRankPoint.newCurrentSeasonRankPoint);
            var query = new StringBuilder();
            query.Append("UPDATE units SET currentRankLevel = ").Append(newCurrentRankLevel)
                .Append(", currentSeasonRankPoint = ").Append(newRankPoint.newCurrentSeasonRankPoint)
                .Append(", nextSeasonRankPoint = ").Append(newRankPoint.newNextSeasonRankPoint)
                .Append(" WHERE userId = ").Append(_userId).Append(" AND id = ").Append(newRankPoint.unitId).Append(";");
            await _userDb.ExecuteNonQueryAsync(query.ToString());
        }
    }
}
