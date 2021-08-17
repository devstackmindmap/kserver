using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using Common;
using Common.Entities.Season;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Clan
{
    public class WebClanJoinByCode : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoAddInvite;

            SeasonUpdator seasonUpdator = new SeasonUpdator(req.UserId);
            var serverCurrentSeason = await seasonUpdator.SeasonUpdateWithTransaction();

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var clan = new ClanJoin(req.UserId, accountDb);
                var clanInfo = await clan.GetClanInfo(req.InviteCode);
                if (clanInfo == null || clanInfo.InviteCode == "0")
                    return new ProtoClanJoinResult { ResultType = AkaEnum.ResultType.Fail };

                ResultType resultType = ResultType.Success;
                await accountDb.BeginTransactionCallback(async () =>
                {
                    resultType = await clan.Join(clanInfo.ClanId, serverCurrentSeason, 0);
                    return true;
                });
                accountDb.Commit();
                if (resultType != AkaEnum.ResultType.Success)
                    return new ProtoClanJoinResult { ResultType = resultType };

                Log.Clan.ClanJoinByCode(req.UserId, clanInfo.ClanId);

                return new ProtoClanJoinResult
                {
                    ResultType = ResultType.Success,
                    ClanInfo = await clan.GetClanProfileAndMembers(clanInfo.ClanId)
                };
            }
        }
    }
}
