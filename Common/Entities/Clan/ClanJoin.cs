using AkaDB.MySql;
using AkaEnum;
using System.Threading.Tasks;

namespace Common
{
    public class ClanJoin : Clan
    {
        public ClanJoin(uint userId, DBContext accountDb) : base(userId, accountDb)
        {
        }

        public async Task<ResultType> Join(uint clanId, uint serverCurrentSeason, int isCnditionCheck)
        {
            var result = await GetDBJobResult(clanId, isCnditionCheck);
            if (result.resultType != ResultType.Success)
                return result.resultType;

            await RedisJob(clanId, result.userRankPoint, serverCurrentSeason, result.countryCode);

            return result.resultType;
        }

        private async Task<(ResultType resultType, int userRankPoint, string countryCode)>  
            GetDBJobResult(uint clanId, int isCnditionCheck)
        {
            var paramInfo = new DBParamInfo();
            paramInfo.SetInputParam(
                new InputArg("$userId", _userId),
                new InputArg("$clanId", clanId),
                new InputArg("$isConditionCheck", isCnditionCheck),
                new InputArg("$fullMemberCount", (int)AkaData.Data.GetConstant(DataConstantType.CLAN_MAX_USER).Value)
                );

            paramInfo.SetOutputParam(
                new OutputArg("$outResultCode", MySql.Data.MySqlClient.MySqlDbType.Int32),
                new OutputArg("$outUserRankPoint", MySql.Data.MySqlClient.MySqlDbType.Int32),
                new OutputArg("$outCountryCode", MySql.Data.MySqlClient.MySqlDbType.String)
                );

            using (await _accountDb.CallStoredProcedureAsync(StoredProcedure.JOIN_CLAN, paramInfo))
            {
                var resultCode = paramInfo.GetOutValue<int>("$outResultCode");

                if (resultCode == 1)
                    return (ResultType.AlreadyClanJoined, 0, "");
                else if (resultCode == 2)
                    return (ResultType.FullClanMember, 0, "");
                else if (resultCode == 3)
                    return (ResultType.PrivateClan, 0, "");
                else if (resultCode == 4)
                    return (ResultType.BelowJoinConditionRankPoint, 0, "");
                else if (resultCode == 5)
                    return (ResultType.ClanNotExist, 0, "");

                var userRankPoint = paramInfo.GetOutValue<int>("$outUserRankPoint");
                var countryCode = paramInfo.GetOutValue<string>("$outCountryCode");

                return (ResultType.Success, userRankPoint, countryCode);
            }
        }

        private async Task RedisJob(uint clanId, int userRankPoint, uint serverCurrentSeason, string countryCode)
        {
            var redis = AkaRedis.AkaRedis.GetDatabase();
            await AkaRedisLogic.GameBattleRankRedisJob.IncrementRankKnightLeagueClanAsync(redis, clanId, userRankPoint, serverCurrentSeason);
            await AkaRedisLogic.GameBattleRankRedisJob.IncrementRankKnightLeagueClanCountryAsync(redis, clanId, userRankPoint, serverCurrentSeason, countryCode);
        }

    }
}
