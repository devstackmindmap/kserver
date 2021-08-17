using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaRedisLogic;
using Common.Entities.Season;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClearTrashDataTool
{
    public class ClearPastRankingBoards
    {
        private IDatabase _redis;
        private uint _basePassSeason;
        private List<string> _countryCodes = new List<string>();

        public ClearPastRankingBoards(IDatabase redis)
        {
            _redis = redis;
        }

        public async Task ClearRankingBoardSeason(uint season)
        {
            await SetData();
            ClearRankingBoard(season);
        }

        public async Task ClearRankingBoardAll()
        {
            await SetData();
            for (uint season = 1; season <= _basePassSeason; season++)
            {
                ClearRankingBoard(season);
            }
        }

        private async Task SetData()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                await GetBasePassSeason(accountDb);
                await GetCountryCodes(accountDb);
            }
        }

        private void ClearRankingBoard(uint season)
        {
            var unitIds = GetUnitsId();
            foreach (var countryCode in _countryCodes)
            {
                _redis.KeyDelete(KeyMaker.GlobalUserRankBoard(season));
                _redis.KeyDelete(KeyMaker.GlobalUserRankBoard(season, countryCode));

                _redis.KeyDelete(KeyMaker.GlobalClanRankBoard(season));
                _redis.KeyDelete(KeyMaker.GlobalClanRankBoard(season, countryCode));


                foreach (var unitId in unitIds)
                {
                    _redis.KeyDelete(KeyMaker.GlobalUnitRankBoard(season, unitId));
                    _redis.KeyDelete(KeyMaker.GlobalUnitRankBoard(season, unitId, countryCode));
                }
            }
        }

        private async Task GetBasePassSeason(DBContext accountDb)
        {
            ServerSeason serverSeason = new ServerSeason(accountDb);
            var serverSeasonInfo = await serverSeason.GetKnightLeagueSeasonInfo();
            _basePassSeason = serverSeasonInfo.CurrentSeason - 5;
        }

        private async Task GetCountryCodes(DBContext accountDb)
        {
            using (var cursor = await accountDb.ExecuteReaderAsync("SELECT country_code FROM _country_codes;"))
            {
                while (cursor.Read())
                    _countryCodes.Add((string)cursor["country_code"]);
            }
        }

        private List<uint> GetUnitsId()
        {
            return Data.GetPrimitiveDict<uint, DataUnit>(DataType.data_unit).Values
                .Where(data => data.UserType == UserType.User)
                .Select(data => data.UnitId).ToList();
        }
    }
}
