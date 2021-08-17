using AkaDB.MySql;
using AkaEnum;
using AkaRedisLogic;
using System.Threading.Tasks;

namespace Common
{
    public class ClanOut : Clan
    {
        public ClanOut(uint userId, DBContext accountDb) : base(userId, accountDb)
        {
        }

        public async Task<(ResultType, uint, int)> Out(uint targetUserId, uint serverCurrentSeason, int isBanish)
        {
            var result = await GetDBJobResult(targetUserId, isBanish);
            if (result.resultType != ResultType.Success)
                return (result.resultType, 0, 0);

            await RedisJob(result.clanId, result.userRankPoint, serverCurrentSeason, result.countryCode, result.isClanDelete);

            return (result.resultType, result.clanId, result.isClanDelete);
        }

        private async Task<(ResultType resultType, int userRankPoint, string countryCode, uint clanId, int isClanDelete)>  
            GetDBJobResult(uint targetId, int isBanish)
        {
            var paramInfo = new DBParamInfo();
            paramInfo.SetInputParam(
                new InputArg("$userId", _userId),
                new InputArg("$targetId", targetId),
                new InputArg("$isBanish", isBanish)
                );

            paramInfo.SetOutputParam(
                new OutputArg("$outResultCode", MySql.Data.MySqlClient.MySqlDbType.Int32),
                new OutputArg("$outUserRankPoint", MySql.Data.MySqlClient.MySqlDbType.Int32),
                new OutputArg("$outCountryCode", MySql.Data.MySqlClient.MySqlDbType.String),
                new OutputArg("$outClanId", MySql.Data.MySqlClient.MySqlDbType.String),
                new OutputArg("$outClanDelete", MySql.Data.MySqlClient.MySqlDbType.Int32)
                );

            using (await _accountDb.CallStoredProcedureAsync(StoredProcedure.OUT_CLAN, paramInfo))
            {
                var resultCode = paramInfo.GetOutValue<int>("$outResultCode");

                if (resultCode == 1)
                    return (ResultType.Fail, 0, "", 0, 0);
                else if (resultCode == 2)
                    return (ResultType.AlreadyTargetOut, 0, "", 0, 0);

                var userRankPoint = paramInfo.GetOutValue<int>("$outUserRankPoint");
                var countryCode = paramInfo.GetOutValue<string>("$outCountryCode");
                var clanId = paramInfo.GetOutValue<uint>("$outClanId");
                var isClanDelete = paramInfo.GetOutValue<int>("$outClanDelete");

                return (ResultType.Success, userRankPoint, countryCode, clanId, isClanDelete);
            }
        }

        private async Task RedisJob(uint clanId, int userRankPoint, uint serverCurrentSeason, string countryCode, int isClanDelete)
        {
            var redis = AkaRedis.AkaRedis.GetDatabase();   
            if (isClanDelete == 1)
            {
                await GameBattleRankRedisJob.RemoveRankKnightLeagueClanAsync(redis, clanId, serverCurrentSeason);
                await GameBattleRankRedisJob.RemoveRankKnightLeagueClanCountryAsync(redis, clanId, serverCurrentSeason, countryCode);
            }
            else
            {
                await GameBattleRankRedisJob.IncrementRankKnightLeagueClanAsync(redis, clanId, userRankPoint * -1, serverCurrentSeason);
                await GameBattleRankRedisJob.IncrementRankKnightLeagueClanCountryAsync(redis, clanId, userRankPoint * -1, serverCurrentSeason, countryCode);
            }                
        }
    }
}
