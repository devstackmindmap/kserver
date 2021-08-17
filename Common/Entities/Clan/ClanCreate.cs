using AkaDB.MySql;
using AkaEnum;
using AkaRedisLogic;
using System.Threading.Tasks;

namespace Common
{
    public class ClanCreate : Clan
    {
        private uint _clanId;
        private int _currentSeasonRankPoint;
        private string _countryCode;

        public ClanCreate(uint userId, DBContext accountDb) : base(userId, accountDb)
        {
        }

        public async Task<ResultType> DBCreateClan(
            string clanName, uint clanSymbolId, ClanPublicType clanPublicType, int joinConditionRankPoint, 
            string countryCode, string clanExplain)
        {
            var paramInfo = new DBParamInfo();
            paramInfo.SetInputParam(
                new InputArg("$userId", _userId),
                new InputArg("$clanName", clanName),
                new InputArg("$clanSymbolId", clanSymbolId),
                new InputArg("$clanPublicType", (int)clanPublicType),
                new InputArg("$joinConditionRankPoint", joinConditionRankPoint),
                new InputArg("$countryCode", countryCode),
                new InputArg("$clanExplain", clanExplain)
                );

            paramInfo.SetOutputParam(
                new OutputArg("$outResultCode", MySql.Data.MySqlClient.MySqlDbType.Int32),
                new OutputArg("$outClanId", MySql.Data.MySqlClient.MySqlDbType.UInt32),
                new OutputArg("$outRankPoint", MySql.Data.MySqlClient.MySqlDbType.Int32)
                ) ;

            using (await _accountDb.CallStoredProcedureAsync(StoredProcedure.CREATE_CLAN, paramInfo))
            {
                var resultCode = paramInfo.GetOutValue<int>("$outResultCode");

                if (resultCode == 1)
                    return ResultType.AlreadyClanJoined;

                if (resultCode == 2)
                    return ResultType.ClanNameDuplicate;

                _clanId = paramInfo.GetOutValue<uint>("$outClanId");
                _currentSeasonRankPoint = paramInfo.GetOutValue<int>("$outRankPoint");
                
                _countryCode = countryCode;

                return ResultType.Success;
            }  
        }

        public uint GetClanId()
        {
            return _clanId;
        }

        public async Task RedisCreateClan(uint serverCurrentSeason)
        {
            var redis = AkaRedis.AkaRedis.GetDatabase();
            await GameBattleRankRedisJob.SetRankKnightLeagueClanAsync(redis, _clanId, _currentSeasonRankPoint, serverCurrentSeason);
            if (false == string.IsNullOrEmpty(_countryCode))
                await GameBattleRankRedisJob.SetRankKnightLeagueClanCountryAsync(redis, _clanId, _currentSeasonRankPoint, serverCurrentSeason, _countryCode);
        }
    }
}
