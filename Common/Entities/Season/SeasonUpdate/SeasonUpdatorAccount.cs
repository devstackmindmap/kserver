using AkaDB.MySql;
using AkaRedisLogic;
using StackExchange.Redis;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Season
{
    public class SeasonUpdatorAccount
    {
        private DBContext _accountDb;
        private uint _userId;
        private SeasonInfo _accountSeasonInfo;
        private uint _serverSeason;
        private IDatabase _redis;
        private StringBuilder _query = new StringBuilder();

        public SeasonUpdatorAccount(DBContext accountDb, IDatabase redis, uint userId, uint serverSeason)
        {
            _accountDb = accountDb;
            _userId = userId;
            _serverSeason = serverSeason;
            _redis = redis;
            GetAccountSeasonInfo().Wait();
        }

        public async Task SeasonUpdate((int newUserCurrentRankData, int newUserNextRankData) newRankPoints)
        {
            _query.Clear();
            _query.Append("UPDATE accounts SET currentSeason = ").Append(_serverSeason)
                .Append(", currentSeasonRankPoint = ").Append(newRankPoints.newUserCurrentRankData)
                .Append(", nextSeasonRankPoint = ").Append(newRankPoints.newUserNextRankData)
                .Append(" WHERE userId = ").Append(_userId).Append(";");
             await _accountDb.ExecuteNonQueryAsync(_query.ToString());

            await SeasonRedisUpdate(newRankPoints.newUserNextRankData);
        }

        private async Task GetAccountSeasonInfo()
        {
            _query.Clear();
            _query.Append("SELECT currentSeason, currentSeasonRankPoint, nextSeasonRankPoint, countryCode " +
                "FROM accounts ")
                .Append("WHERE userId=").Append(_userId).Append(";");

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    throw new Exception("Check the data");

                _accountSeasonInfo = new SeasonInfo
                {
                    currentSeason = (int)cursor["currentSeason"],
                    currentSeasonRankPoint = (int)cursor["currentSeasonRankPoint"],
                    nextSeasonRankPoint = (int)cursor["nextSeasonRankPoint"],
                    CountryCode = (string)cursor["countryCode"]
                };
            }
        }

        public bool IsCurrentSeason()
        {
            return _serverSeason == _accountSeasonInfo.currentSeason;
        }

        public bool IsSeasonChange()
        {
            return _serverSeason - 1 == _accountSeasonInfo.currentSeason;
        }

        public int CurrentSeason => _accountSeasonInfo?.currentSeason ?? 0;

        private async Task SeasonRedisUpdate(int newNextSeasonRankPoint)
        {
            if (newNextSeasonRankPoint > 0)
            {
                await GameBattleRankRedisJob.SetRankKnightLeagueUserAsync(_redis, _userId, newNextSeasonRankPoint, _serverSeason + 1);
                await GameBattleRankRedisJob.SetRankKnightLeagueUserCountryAsync(_redis, _userId, newNextSeasonRankPoint, _serverSeason + 1, _accountSeasonInfo.CountryCode);
            }
        }

        public string GetCountryCode()
        {
            return _accountSeasonInfo.CountryCode;
        }
    }
}
