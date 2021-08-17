using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaRedisLogic;
using AkaUtility;
using System;
using System.Threading.Tasks;

namespace Common.Entities.User
{
    public class CountryChanger : UserAdditionalInfo
    {
        public CountryChanger(DBContext accountDb, DBContext userDb, uint userId, UserAdditionalInfoType userInfoType) : base(accountDb, userDb, userId, userInfoType)
        {

        }

        public override async Task<ResultType> Change(RequestValue requestValue)
        {
            var recentCountryChangeDateTime = (await GetAdditionalUserInfo()).RecentDateTimeCountryChange;
            var changeWaitingMinute = (int)Data.GetConstant(DataConstantType.WAITING_TIME_LOCAL_STATION_CHANGE).Value;
            if (recentCountryChangeDateTime.AddMinutes(changeWaitingMinute) > DateTime.UtcNow)
                return ResultType.Fail;

            
            await SetValue();
            await UpdateCountryRedis(requestValue);
            await UpdateCountry(requestValue);
            return ResultType.Success;
        }

        private async Task SetValue()
        {
            var utcNow = DateTime.UtcNow.ToTimeString();
            _query.Clear();
            _query.Append("INSERT INTO user_info (userId, recentDateTimeCountryChange) VALUES (")
                .Append(_userId).Append(", '").Append(utcNow)
                .Append("') ON DUPLICATE KEY UPDATE recentDateTimeCountryChange = '").Append(utcNow).Append("';");
            await _userDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private async Task UpdateCountry(RequestValue requestValue)
        {
            _query.Clear();
            _query.Append("UPDATE accounts SET countryCode = '")
                .Append(requestValue.StringValue)
                .Append("' WHERE userId = ")
                .Append(_userId).Append(";");

            await _accountDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private async Task UpdateCountryRedis(RequestValue requestValue)
        {
            uint currentSeason;
            int currentSeasonRankPoint;
            int nextSeasonRankPoint;
            string countryCode;
            uint clanId;
            var redis = AkaRedis.AkaRedis.GetDatabase();

            using (var cursor = await GetAccountInfo())
            {
                if (false == cursor.Read())
                    return;

                currentSeason = (uint)cursor["currentSeason"];
                currentSeasonRankPoint = (int)cursor["currentSeasonRankPoint"];
                nextSeasonRankPoint = (int)cursor["nextSeasonRankPoint"];
                countryCode = (string)cursor["countryCode"];

                await GameBattleRankRedisJob.RemoveRankKnightLeagueUserCountryAsync(redis, _userId, currentSeason, countryCode);
                await GameBattleRankRedisJob.RemoveRankKnightLeagueUserCountryAsync(redis, _userId, currentSeason + 1, countryCode);

                await GameBattleRankRedisJob.SetRankKnightLeagueUserCountryAsync(redis, _userId, currentSeasonRankPoint, currentSeason, requestValue.StringValue);
                await GameBattleRankRedisJob.SetRankKnightLeagueUserCountryAsync(redis, _userId, nextSeasonRankPoint, currentSeason + 1, requestValue.StringValue);
            }

            _query.Clear();
            _query.Append("SELECT clanId FROM clan_members WHERE userId = ").Append(_userId).Append(";");

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                clanId = 0;
                if (cursor.Read())
                {
                    clanId = (uint)cursor["clanId"];
                }
            }

            if (clanId > 0)
            {
                await GameBattleRankRedisJob.RemoveRankKnightLeagueClanCountryAsync(redis, clanId, currentSeason, countryCode);
                await GameBattleRankRedisJob.SetRankKnightLeagueClanCountryAsync(redis, clanId, currentSeasonRankPoint, currentSeason, requestValue.StringValue);
            }

            _query.Clear();
            _query.Append("SELECT id, currentSeasonRankPoint, nextSeasonRankPoint FROM units WHERE userId = ")
                .Append(_userId).Append(";");
            using (var cursor = await _userDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    var unitId = (uint)cursor["id"];
                    currentSeasonRankPoint = (int)cursor["currentSeasonRankPoint"];
                    nextSeasonRankPoint = (int)cursor["nextSeasonRankPoint"];
                    await GameBattleRankRedisJob.RemoveRankKnightLeagueUnitCountryAsync(redis, _userId, unitId, currentSeason, countryCode);
                    await GameBattleRankRedisJob.RemoveRankKnightLeagueUnitCountryAsync(redis, _userId, unitId, currentSeason + 1, countryCode);
                    await GameBattleRankRedisJob.SetRankKnightLeagueUnitCountryAsync(redis, _userId, unitId, currentSeasonRankPoint, currentSeason, requestValue.StringValue);
                    await GameBattleRankRedisJob.SetRankKnightLeagueUnitCountryAsync(redis, _userId, unitId, nextSeasonRankPoint, currentSeason + 1, requestValue.StringValue);
                }
            }
        }
    }
}
