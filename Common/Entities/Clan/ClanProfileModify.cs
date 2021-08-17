using AkaDB.MySql;
using AkaEnum;
using AkaRedisLogic;
using System.Threading.Tasks;

namespace Common
{
    public class ClanProfileModify : Clan
    {
        private uint _clanId;
        private int _currentSeasonRankPoint;
        private string _oldCountryCode;
        private string _countryCode;

        public ClanProfileModify(uint userId, DBContext accountDb) : base(userId, accountDb)
        {
        }

        public async Task<ResultType> DBModifyClan(
            uint clanId, uint clanSymbolId, ClanPublicType clanPublicType, int joinConditionRankPoint, 
            string countryCode, string clanExplain)
        {
            var paramInfo = new DBParamInfo();
            paramInfo.SetInputParam(
                new InputArg("$userId", _userId),
                new InputArg("$clanId", clanId),
                new InputArg("$clanSymbolId", clanSymbolId),
                new InputArg("$clanPublicType", (int)clanPublicType),
                new InputArg("$joinConditionRankPoint", joinConditionRankPoint),
                new InputArg("$countryCode", countryCode),
                new InputArg("$clanExplain", clanExplain)
                );

            paramInfo.SetOutputParam(
                new OutputArg("$outResultCode", MySql.Data.MySqlClient.MySqlDbType.Int32),
                new OutputArg("$outRankPoint", MySql.Data.MySqlClient.MySqlDbType.Int32),
                new OutputArg("$oldCountryCode", MySql.Data.MySqlClient.MySqlDbType.String)
                ) ;

            using (await _accountDb.CallStoredProcedureAsync(StoredProcedure.MODIFY_PROFILE_CLAN, paramInfo))
            {
                var resultCode = paramInfo.GetOutValue<int>("$outResultCode");

                if (resultCode == 1)
                    return ResultType.Fail;

                _clanId = clanId;
                _currentSeasonRankPoint = paramInfo.GetOutValue<int>("$outRankPoint");
                _oldCountryCode = paramInfo.GetOutValue<string>("$oldCountryCode");
                _countryCode = countryCode;

                return ResultType.Success;
            }  
        }

        public async Task RedisModify(uint serverCurrentSeason)
        {
            var redis = AkaRedis.AkaRedis.GetDatabase();
            await GameBattleRankRedisJob.SetRankKnightLeagueClanAsync(redis, _clanId, _currentSeasonRankPoint, serverCurrentSeason);
            if (false == string.IsNullOrEmpty(_countryCode) && (_countryCode != _oldCountryCode))
            {
                await GameBattleRankRedisJob.RemoveRankKnightLeagueClanCountryAsync(redis, _clanId, serverCurrentSeason, _oldCountryCode);
                await GameBattleRankRedisJob.SetRankKnightLeagueClanCountryAsync(redis, _clanId, _currentSeasonRankPoint, serverCurrentSeason, _countryCode);
            }
                
        }
    }
}
