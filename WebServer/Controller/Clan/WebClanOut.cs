using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using Common;
using Common.Entities.Season;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Clan
{
    public class WebClanOut : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserId;

            SeasonUpdator seasonUpdator = new SeasonUpdator(req.UserId);
            var serverCurrentSeason = await seasonUpdator.SeasonUpdateWithTransaction();

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var clan = new ClanOut(req.UserId, accountDb);
               
                (ResultType resultType, uint clanId, int isClanDelete) result = (ResultType.Success, 0, 0);
                await accountDb.BeginTransactionCallback(async () =>
                {
                    result = await clan.Out(req.UserId, serverCurrentSeason, 0);
                    return true;
                });
                accountDb.Commit();

                if (result.resultType != ResultType.Success)
                    return new ProtoClanOutResult { ResultType = result.resultType };

                Log.Clan.ClanOut(req.UserId, result.clanId);

                if (result.isClanDelete == 1)
                    Log.Clan.ClanDelete(req.UserId, result.clanId);


                var clanRecommend = new ClanRecommend(req.UserId, accountDb);
                return new ProtoClanOutResult
                {
                    ResultType = ResultType.Success,
                    ClanInfos = await clanRecommend.GetRecommendClans()
                };
            }
        }
    }
}
