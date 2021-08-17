using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using Common;
using Common.Entities.Season;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Clan
{
    public class WebClanJoin : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserIdTargetId;

            SeasonUpdator seasonUpdator = new SeasonUpdator(req.UserId);
            var serverCurrentSeason = await seasonUpdator.SeasonUpdateWithTransaction();

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var clan = new ClanJoin(req.UserId, accountDb);
                var clanInfo = await clan.GetClanInfo(req.TargetId);
                if (clanInfo == null)
                    return new ProtoClanJoinResult { ResultType = AkaEnum.ResultType.ClanNotExist };

                ResultType resultType = ResultType.Success;
                await accountDb.BeginTransactionCallback(async () =>
                {
                    resultType = await clan.Join(clanInfo.ClanId, serverCurrentSeason, 1);
                    return true;
                });
                accountDb.Commit();

                if (resultType != AkaEnum.ResultType.Success)
                    return new ProtoClanJoinResult { ResultType = resultType };

                Log.Clan.ClanJoin(req.UserId, clanInfo.ClanId);

                return new ProtoClanJoinResult
                {
                    ResultType = AkaEnum.ResultType.Success,
                    ClanInfo = await clan.GetClanProfileAndMembers(clanInfo.ClanId)
                };
            }
        }
    }
}
