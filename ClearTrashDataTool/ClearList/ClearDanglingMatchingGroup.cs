using AkaConfig;
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
    public class ClearDanglingMatchingGroup
    {
        private IDatabase _redis;
        private int _basePassSeason;
        private List<string> _countryCodes = new List<string>();
        private List<int> _groupCodes = new List<int>();

        private List<string> _tierMatchingIds = new List<string>();

        public ClearDanglingMatchingGroup(IDatabase redis)
        {
            _redis = redis;
        }


        public async Task ClearMatchingGroupAll()
        {
            await SetData();

            //Matching [group][index][tier]
            var matchingGroupScoreList = _groupCodes.SelectMany(
                groupCode => Config.MatchingServerConfig.MatchingServerList[groupCode].SelectMany(
                    matchingLine => _tierMatchingIds.Select(
                        tierMatchingId => $"{RedisKeyType.ZMatchingGroup.ToString()}[{groupCode.ToString()}][{matchingLine.Key}]{tierMatchingId.ToString()}")));

            await Task.WhenAll(matchingGroupScoreList.Select(matchingGroupScore => _redis.KeyDeleteAsync(matchingGroupScore)));

            //Global variable
            var roomIdList = RedisKeyType.HRoomIdList.ToString();
            var matchingSessionInfo = RedisKeyType.HMatchingSessionInfo.ToString();

            await _redis.KeyDeleteAsync(roomIdList);
            await _redis.KeyDeleteAsync(matchingSessionInfo);

            //Friend Matching 
            var friendMatchingGroup = RedisKeyType.ZFvFMatchingGroup.ToString() + "*";

            await _redis.KeyDeleteAsync(friendMatchingGroup);

        }

        private async Task SetData()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                await GetCountryCodes(accountDb);
                await GetGroupCodes(accountDb);
                _tierMatchingIds = Data.GetPrimitiveDict<int, DataRankTierMatching>(DataType.data_rank_tier_matching).Keys.Select(key => key.ToString()).ToList();
            }
        }

        private async Task GetCountryCodes(DBContext accountDb)
        {
            using (var cursor = await accountDb.ExecuteReaderAsync("SELECT country_code FROM _country_codes;"))
            {
                while (cursor.Read())
                    _countryCodes.Add((string)cursor["country_code"]);
            }
        }

        private async Task GetGroupCodes(DBContext accountDb)
        {
            using (var cursor = await accountDb.ExecuteReaderAsync("SELECT DISTINCT group_code FROM _ip2location where country_code in ( '"  +  string.Join("','",_countryCodes) + "' );" ))
            {
                while (cursor.Read())
                    _groupCodes.Add((int)cursor["group_code"]);
            }
        }

    }
}
