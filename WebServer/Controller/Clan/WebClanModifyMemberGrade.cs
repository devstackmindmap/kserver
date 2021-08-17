using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using Common;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Clan
{
    public class WebClanModifyMemberGrade : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoModifyMemberGrade;
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                ResultType resultType = ResultType.Success;
                await accountDb.BeginTransactionCallback(async () =>
                {
                    var clan = new ClanModifyMemberGrade(req.UserId, accountDb);
                    resultType = await clan.SetMemberGrade(req.TargetId, req.ClanMemberGrade);
                    return true;
                });

                Log.Clan.ClanModifyGrade(req.UserId, req.TargetId, req.ClanMemberGrade);

                return new ProtoResult
                {
                    ResultType = resultType
                };
            }
        }
    }
}
