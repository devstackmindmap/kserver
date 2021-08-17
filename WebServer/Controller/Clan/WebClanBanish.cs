using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using Common;
using Common.Entities.Season;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Clan
{
    public class WebClanBanish : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserIdTargetId;

            SeasonUpdator seasonUpdator = new SeasonUpdator(req.TargetId);
            var serverCurrentSeason = await seasonUpdator.SeasonUpdateWithTransaction();

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var clan = new ClanOut(req.UserId, accountDb);
               
                (ResultType resultType, uint clanId, int isClanDelete) result = (ResultType.Success, 0, 0);
                await accountDb.BeginTransactionCallback(async () =>
                {
                    result = await clan.Out(req.TargetId, serverCurrentSeason, 1);
                    return true;
                });
                accountDb.Commit();

                if (result.resultType != ResultType.Success)
                    return new ProtoResult { ResultType = result.resultType };

                Log.Clan.ClanBanish(req.UserId, result.clanId, req.TargetId);

                return new ProtoResult
                {
                    ResultType = ResultType.Success
                };
            }
        }
    }
}
