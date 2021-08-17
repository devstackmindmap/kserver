using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using Common;
using Common.Entities.Season;
using CommonProtocol;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Controller.Clan
{
    public class WebClanCreate : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoClanCreate;
            if (await IsNotEnoughUserLevel(req.UserId))
                return new ProtoOnClanCreate { ResultType = ResultType.NotEnoughUserLevel };

            var slangFilter = new AkaUtility.SlangFilter();
            if (slangFilter.IsFiltered(req.ClanName))
                return new ProtoOnClanCreate { ResultType = ResultType.Slang };

            uint serverCurrentSeason = 0;
            ResultType resultType = ResultType.Success;

            if (req.JoinConditionRankPoint > AkaData.Data.GetConstant(DataConstantType.CLAN_SUBSCRIPTION_LIMIT_MAX_RANK_POINT).Value)
                return new ProtoOnClanCreate { ResultType = ResultType.Fail };

            SeasonUpdator seasonUpdator = new SeasonUpdator(req.UserId);
            serverCurrentSeason = await seasonUpdator.SeasonUpdateWithTransaction();

            uint clanId = 0;
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var createClan = new ClanCreate(req.UserId, accountDb);
                await accountDb.BeginTransactionCallback(async () =>
                {
                    resultType = await createClan.DBCreateClan(
                        req.ClanName, req.ClanSymbolId, req.ClanPublicType, req.JoinConditionRankPoint, 
                        req.CountryCode, req.ClanExplain);

                    return true;
                });

                accountDb.Commit();

                if (resultType == ResultType.Success)
                    await createClan.RedisCreateClan(serverCurrentSeason);

                clanId = createClan.GetClanId();

                Log.Clan.ClanCreate(req.UserId, clanId, req.ClanName, req.ClanSymbolId, req.ClanPublicType, 
                    req.JoinConditionRankPoint, req.CountryCode, req.ClanExplain);

                return new ProtoOnClanCreate
                {
                    ResultType = resultType,
                    ClanInfo = await createClan.GetClanProfileAndMembers(clanId)
                };
            }
        }

        private async Task<bool> IsNotEnoughUserLevel(uint userId)
        {
            using (var userDb = new DBContext(userId))
            {
                var query = new StringBuilder();
                query.Append("SELECT level FROM users WHERE userId = ").Append(userId).Append(";");
                using (var cursor = await userDb.ExecuteReaderAsync(query.ToString()))
                {
                    if (false == cursor.Read())
                        throw new System.Exception("Wrong Access");

                    if ((uint)Data.GetConstant(DataConstantType.MIN_USER_LEVEL_TO_CREATE_CLAN).Value > (uint)cursor["level"])
                        return true;
                }
            }
            return false;
        }
    }
}
