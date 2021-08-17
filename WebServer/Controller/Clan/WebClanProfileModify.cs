using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using Common;
using Common.Entities.Season;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Clan
{
    public class WebClanProfileModify : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoClanProfileModify;
            uint serverCurrentSeason = 0;
            ResultType resultType = ResultType.Success;

            if (req.JoinConditionRankPoint > AkaData.Data.GetConstant(DataConstantType.CLAN_SUBSCRIPTION_LIMIT_MAX_RANK_POINT).Value)
                return new ProtoOnClanCreate { ResultType = ResultType.Fail };

            SeasonUpdator seasonUpdator = new SeasonUpdator(req.UserId);
            serverCurrentSeason = await seasonUpdator.SeasonUpdateWithTransaction();

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var clan = new ClanProfileModify(req.UserId, accountDb);
                await accountDb.BeginTransactionCallback(async () =>
                {
                    resultType = await clan.DBModifyClan(
                        req.ClanId, req.ClanSymbolId, req.ClanPublicType, req.JoinConditionRankPoint, 
                        req.CountryCode, req.ClanExplain);
                    
                    return true;
                });
                accountDb.Commit();

                if (resultType == ResultType.Success)
                    await clan.RedisModify(serverCurrentSeason);

                Log.Clan.ClanProfileModify(req.UserId, req.ClanId, req.ClanSymbolId, req.ClanPublicType,
                    req.JoinConditionRankPoint, req.CountryCode, req.ClanExplain);

                return new ProtoResult{ ResultType = resultType };
            }
        }
    }
}
